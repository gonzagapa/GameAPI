using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameStore.Dtos;
using GameStore.Models;
using GameStore.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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

                return Results.Ok(user); 
            });

            app.MapPost("/login", async (UserDto userDto, IAuthService authService) =>
            {
               var token = await authService.LoginAsync(userDto);
               return String.IsNullOrEmpty(token) ? Results.BadRequest("User or password isnt correct"): Results.Ok(token);
            }); 
        } 
    }
}