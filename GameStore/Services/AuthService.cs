using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameStore.Data;
using GameStore.Dtos;
using GameStore.Models;
using GameStore.Repository;
using GameStore.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameStore.Services
{
    public class AuthService :Repository<User>, IAuthService
    {
        private readonly IConfiguration _configuration; 

        public AuthService(IConfiguration configuration,GameStoreContext dbContext)
        :base(dbContext)
        {
            _configuration = configuration;
            
        }
        public async Task<string> LoginAsync(UserDto userDto)
        {
                if(!_dbContext.User.Any(user => user.Username == userDto.Username))
                {
                    return string.Empty;
                } 

                var user = await _dbContext.User.FirstOrDefaultAsync(user => user.Username == userDto.Username);
                if(user is null) return string.Empty;

                if(new PasswordHasher<User>().VerifyHashedPassword(user, user.HashPassword, userDto.Password) == PasswordVerificationResult.Failed)
                {
                   return string.Empty;
                }

                string token = CreateToken(user, _configuration); 
                return token;
        }

        public async Task<User?> RegisterAsync(UserDto userDto)
        {
            if(_dbContext.User.Any(user => user.Username == userDto.Username))
            {
                return null;
            }
            User user = new();

            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user,userDto.Password); 

            user.Username = userDto.Username; 
            user.HashPassword = hashedPassword; 

            await AddAsync(user);
            return user;
        } 

        private static string CreateToken(User user, IConfiguration configuration)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role,user.Role)
            }; 

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token") ?? "")); 

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