using System.Security.Claims;
using GameStore.Dtos;
using GameStore.Mapping;
using GameStore.Services.Interface;

namespace GameStore.Endpoints {
    public static class UserEndponts
    {
        public static void MapUserEndponts(this WebApplication app)
        {
            app.MapPost("/register", async (UserDto userDto, IAuthService authService) =>
            {
                var user = await authService.RegisterAsync(userDto);
                if(user is null)
                {
                    return Results.BadRequest("User exist");
                }

                return Results.Ok(user.MappingRegisterDto()); 
            }).AllowAnonymous();

            app.MapPost("/login", async (UserDto userDto, IAuthService authService) =>
            {
               var response = await authService.LoginAsync(userDto);
               return  response is null ? Results.BadRequest("User or password isnt correct"): Results.Ok(response);
            }).AllowAnonymous();

            app.MapGet("/admin-only", () =>
            {
                return Results.Ok("you are an admin");
            }).RequireAuthorization("Admin");

            app.MapPost("/refresh-token", async (RefreshTokenDto request, IAuthService authService) =>
            {
                var response =  await authService.RefreshTokenAsync(request); 
                if(response is null)
                {
                    return Results.BadRequest("Your refresh token is still valid or your reques body is wrong ");
                } 
                return Results.Ok(response); 
            }).AllowAnonymous();

            app.MapPost("/logout", async (ClaimsPrincipal user, IAuthService authService) =>
            {
                var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var response = await authService.LogoutAsync(userId); 
                if(response is null)
                {
                    return Results.BadRequest($"{userId} isnt a valid id");
                } 
                return Results.NoContent(); 
            }).RequireAuthorization();
        } 
    }
}