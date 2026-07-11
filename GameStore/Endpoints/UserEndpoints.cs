using System.Security.Claims;
using GameStore.Dtos;
using GameStore.Mapping;
using GameStore.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameStore.Endpoints {
    public static class UserEndponts
    {
        public static void MapUserEndponts(this WebApplication app)
        {
            app.MapPost("/register", 
            async Task<Results<BadRequest<string>,Ok<UserRegisterDto> >> 
            (UserDto userDto, IAuthService authService) =>
            {
                var user = await authService.RegisterAsync(userDto);
                if(user is null)
                {
                    return TypedResults.BadRequest("User doesn't exist");
                }

                return TypedResults.Ok(user.MappingRegisterDto()); 

            }).AllowAnonymous();

            app.MapPost("/login", 
            async Task<Results<BadRequest<string>, Ok<TokenResponseDto> > >
            (UserDto userDto, IAuthService authService) =>
            {
               var response = await authService.LoginAsync(userDto);
               return  response is null ? 
                TypedResults.BadRequest("User or password isnt correct"): TypedResults.Ok(response);

            }).AllowAnonymous();

            // app.MapGet("/admin-only", () =>
            // {
            //     return Results.Ok("you are an admin");
            // }).RequireAuthorization("Admin");

            app.MapPost("/refresh-token", 
            async  Task<Results<BadRequest<string>, Ok<TokenResponseDto> > >
            (RefreshTokenDto request, IAuthService authService) =>
            {
                var response =  await authService.RefreshTokenAsync(request); 
                if(response is null)
                {
                    return TypedResults.BadRequest("Your refresh token is still valid or your reques body is wrong ");
                } 
                return TypedResults.Ok(response); 
            }).AllowAnonymous();



            app.MapPost("/logout", 
            async Task<Results<BadRequest<string>, NoContent>>
            (ClaimsPrincipal user, IAuthService authService) =>
            {
                var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var response = await authService.LogoutAsync(userId); 
                if(response is null)
                {
                    return TypedResults.BadRequest($"{userId} isnt a valid id");
                } 
                return TypedResults.NoContent(); 
            }).RequireAuthorization();
        } 
    }
}