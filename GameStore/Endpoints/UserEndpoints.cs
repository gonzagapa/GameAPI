using GameStore.Dtos;
using GameStore.Models;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Endpoints {
    public static class UserEndponts
    {
        public static User user = new();
        public static void MapUserEndponts(this WebApplication app)
        {
            app.MapPost("/register", (UserDto userDto) =>
            {
                var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user,userDto.Password); 

                user.Username = userDto.Username; 
                user.HashPassword = hashedPassword; 
            });

            app.MapPost("/login", () =>
            {
                
            });
        }
    }
}