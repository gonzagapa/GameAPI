# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Run the application
dotnet run --project GameStore/GameStore.csproj

# Build
dotnet build

# Add a new EF Core migration
dotnet ef migrations add <MigrationName> --project GameStore/GameStore.csproj

# Apply pending migrations manually (also runs automatically on startup via MigrateDB())
dotnet ef database update --project GameStore/GameStore.csproj
```

API documentation is available at `/scalar/v1` in development mode (Scalar UI).

## Architecture

This is a .NET 10 Minimal API with JWT authentication, EF Core (SQLite), and the Repository pattern.

### Request flow

```
HTTP Request → Endpoint (MapXxxEndpoints) → Repository or Service → EF Core → SQLite
```

### Key layers

- **Endpoints** (`Endpoints/`) — Minimal API route definitions registered as extension methods on `WebApplication`. Each file groups a resource: `GameEndpoints`, `GenreEndpoints`, `UserEndpoints`.
- **Repository** (`Repository/`) — Generic `Repository<T>` base class handles CRUD. `GameRepository` extends it with game-specific queries (pagination, summary projection). `AuthService` also inherits from `Repository<User>`.
- **Services** (`Services/AuthService.cs`) — Auth logic: register, login, refresh token. Issues JWT access tokens (1-day expiry) and random refresh tokens (7-day expiry) stored in the User table.
- **Data** (`Data/`) — `GameStoreContext` (EF DbContext) and `DataExtension` (registers SQLite, seeds genres/games, runs migrations on startup via `app.MigrateDB()`).
- **DTOs** (`Dtos/`) — Record types for all request/response shapes. Never expose `Models` directly from endpoints.

### Auth flow

1. `POST /register` / `POST /login` → returns `TokenResponseDto` (`{ accessToken, refreshToken }`)
2. Access token is a signed JWT (HS512) with `Name` + `Role` claims, valid 1 day
3. `POST /refresh-token` with `{ userId, refreshToken }` → issues a new token pair
4. Write endpoints (`POST/PUT/DELETE /games`) require `RequireAuthorization()`
5. `GET /admin-only` requires the `"Admin"` policy (role = `"Admin"`)

### Configuration

JWT settings live in `AppSettings` section (`appsettings.Development.json`):
- `AppSettings:Token` — signing key (must be long and random in production)
- `AppSettings:Issuer` / `AppSettings:Audience`

Database connection string: `ConnectionStrings:GameStore` → `Data Source=GameStore.db` (SQLite file in project root).

### Pagination

`GET /games` supports offset-based pagination via `?pageNumber=1&pageSize=5`. Page size is capped at 30 and defaults to 5. Returns `PageResponseOffsetDto<Game>` with total pages computed server-side.
