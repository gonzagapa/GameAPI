using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameStore.Dtos;
using GameStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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

                return Results.Ok(user); 
            });

            app.MapPost("/login", (UserDto userDto, IConfiguration configuration) =>
            {
                if(user.Username != userDto.Username)
                {
                    return Results.BadRequest("User o password not found");
                } 

                if(new PasswordHasher<User>().VerifyHashedPassword(user, user.HashPassword, userDto.Password) == PasswordVerificationResult.Failed)
                {
                    return Results.BadRequest("User o password not found");
                }

                string token = CreateToken(user, configuration); 
                return Results.Ok(token); 
            }); 
        } 

        private static string CreateToken(User user, IConfiguration configuration)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            }; 

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token"))); 

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"), 
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1), 
                signingCredentials:creds 
            ); 

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }
    }
}