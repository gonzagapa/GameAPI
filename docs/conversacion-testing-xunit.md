# Conversación: Testing con xUnit en GameStore

## ¿Vale la pena escribir testing con xUnit en esta API sencilla?

Sí vale la pena, aunque no hace falta cobertura exhaustiva. En una API como esta el mayor riesgo no está en el CRUD trivial sino en la lógica que sí tiene ramas: `AuthService` (hash de password, expiración de tokens, refresh/logout), la paginación de `GameRepository`, y el mapping de DTOs (que el password hash no se filtre en `UserRegisterDto`). Esos puntos son baratos de romper sin darte cuenta al refactorizar y difíciles de verificar a mano cada vez.

Prioridad sugerida:
1. **Unit tests** con xUnit + Moq/NSubstitute para `AuthService` y la lógica de paginación — son puros, rápidos, sin dependencias de infra.
2. **Integration tests** con `WebApplicationFactory` + EF Core InMemory (o SQLite in-memory) para 2-3 endpoints críticos (login, refresh-token, un endpoint protegido) — validan que `UseAuthentication`/`UseAuthorization` y las políticas de rol funcionan de punta a punta.
3. Evitar testear los CRUD triviales de `Genre`/`Game` uno por uno si no tienen lógica propia — ahí el ROI es bajo.

Ya se tenía en el repo (según git log) la carpeta `Endpoints/` de test y el paquete de EF InMemory instalado.

---

## Paso a paso: implementar unit tests para `AuthService`

Con este código lo más práctico para aprender es testear `AuthService` usando **EF Core InMemory** como base de datos falsa (ya está referenciado transitivamente vía `GameStore.csproj`, no hace falta instalar nada más). No conviene mockear `GameStoreContext` directamente porque `Repository<T>` lo usa como clase concreta, no como interfaz — sería mucho esfuerzo para poco beneficio en un proyecto de este tamaño.

### Paso 1 — Crear la carpeta y el archivo de test

`GameStore.Test/Services/AuthServiceTest.cs`

### Paso 2 — Armar un helper para crear un contexto aislado por test

Cada test necesita su propia base de datos en memoria (si comparten nombre, un test contamina al otro):

```csharp
using GameStore.Data;
using GameStore.Dtos;
using GameStore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GameStore.Test.Services;

public class AuthServiceTest
{
    private static GameStoreContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GameStoreContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GameStoreContext(options);
    }

    private static IConfiguration CreateConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            ["AppSettings:Token"] = "test-signing-key-that-is-long-enough-1234567890",
            ["AppSettings:Issuer"] = "TestIssuer",
            ["AppSettings:Audience"] = "TestAudience"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }
}
```

### Paso 3 — Escribir el primer test siguiendo el patrón AAA (Arrange / Act / Assert)

```csharp
    [Fact]
    public async Task RegisterAsync_should_create_user_with_hashed_password()
    {
        // Arrange
        using var context = CreateContext();
        var service = new AuthService(CreateConfiguration(), context);
        var dto = new UserDto("gonzalo", "MyPassword123!");

        // Act
        var user = await service.RegisterAsync(dto);

        // Assert
        Assert.NotNull(user);
        Assert.Equal("gonzalo", user!.Username);
        Assert.NotEqual(dto.Password, user.HashPassword); // nunca guardamos el password en texto plano
    }

    [Fact]
    public async Task RegisterAsync_should_return_null_when_username_already_exists()
    {
        // Arrange
        using var context = CreateContext();
        var service = new AuthService(CreateConfiguration(), context);
        var dto = new UserDto("gonzalo", "MyPassword123!");
        await service.RegisterAsync(dto);

        // Act
        var secondAttempt = await service.RegisterAsync(dto);

        // Assert
        Assert.Null(secondAttempt);
    }

    [Fact]
    public async Task LoginAsync_should_return_token_response_with_valid_credentials()
    {
        // Arrange
        using var context = CreateContext();
        var service = new AuthService(CreateConfiguration(), context);
        var dto = new UserDto("gonzalo", "MyPassword123!");
        await service.RegisterAsync(dto);

        // Act
        var response = await service.LoginAsync(dto);

        // Assert
        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(response.RefreshToken));
    }

    [Fact]
    public async Task LoginAsync_should_return_null_with_wrong_password()
    {
        // Arrange
        using var context = CreateContext();
        var service = new AuthService(CreateConfiguration(), context);
        await service.RegisterAsync(new UserDto("gonzalo", "MyPassword123!"));

        // Act
        var response = await service.LoginAsync(new UserDto("gonzalo", "WrongPassword"));

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task LoginAsync_should_return_null_when_user_does_not_exist()
    {
        // Arrange
        using var context = CreateContext();
        var service = new AuthService(CreateConfiguration(), context);

        // Act
        var response = await service.LoginAsync(new UserDto("nobody", "whatever"));

        // Assert
        Assert.Null(response);
    }
```

*(Ajustar `UserDto`/`TokenResponseDto` a los nombres reales de propiedades si difieren.)*

### Paso 4 — Correr los tests

```bash
dotnet test
```

o solo esta clase:

```bash
dotnet test --filter FullyQualifiedName~AuthServiceTest
```

### Paso 5 — Ideas para seguir practicando

- `LogoutAsync`: registrar, loguear (para generar refresh token), luego logout, y verificar que `user.RefreshToken` quede en `null`.
- `RefreshTokenAsync`: caso con token válido, caso con token expirado (setear `RefreshTokenExpiryTime` en el pasado manualmente antes de llamar), caso con token incorrecto.

### Por qué este enfoque (y no mocks)

- **Arrange/Act/Assert** es la estructura estándar: preparar datos, ejecutar la acción, verificar el resultado.
- Usar InMemory en vez de mockear el `DbContext` es más simple de aprender y prueba el flujo real (guardar, buscar, actualizar), que es justo donde vive la lógica de `AuthService`.
- Un `Guid` como nombre de base por test evita que un test filtre datos a otro — es el error más común al empezar con EF InMemory.

---

## ¿No es peligroso agregar la configuración de AppSettings para los tests en texto fijo?

No, no es peligroso — y de hecho es la práctica recomendada. La diferencia clave es **cuál** clave se pone en texto plano:

- **Peligroso**: copiar la clave real de `appsettings.Development.json` (o de producción) dentro del test.
- **Lo correcto**: usar una clave dummy inventada, sin relación con ninguna clave real. Solo sirve para que `JwtSecurityTokenHandler` pueda firmar y el test corra — el token generado nunca sale del proceso.

Razones:
1. Los tests no comparten secretos con producción.
2. El token generado en el test nunca se valida contra ningún servidor real.
3. Si se rota la clave real, los tests no se rompen.

Cuidado real: nunca commitear la clave real de `AppSettings:Token` en un test o en el repo — eso queda en el historial de git para siempre.

---

## Revisión de `appsettings.Development.json`

Se encontró que el archivo **ya no está trackeado** actualmente (se agregó a `.gitignore` en el commit `78ff407`), pero el commit anterior `3eee986` sí lo commiteó con una clave real:

```
"Token": "MySuperSecureAndRandomKeyThatLooksJustAwesomeAndNeedsToBeVeryVeryLong!!!111oneeleven"
```

Ese valor sigue visible en el historial de git (`git show 3eee986:GameStore/appsettings.Development.json`).

**Conclusión del usuario:** este repo no estará en un ambiente de producción, por lo tanto el riesgo práctico es prácticamente nulo — la clave nunca protegió nada real, y reescribir el historial de git para un proyecto de aprendizaje sería un esfuerzo desproporcionado al problema. No hace falta ninguna acción adicional; mantener `appsettings.Development.json` en `.gitignore` es suficiente buen hábito a futuro.