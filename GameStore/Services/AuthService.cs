using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        public async Task<TokenResponseDto?> LoginAsync(UserDto userDto)
        {
                if(!_dbContext.User.Any(user => user.Username == userDto.Username))
                {
                    return null;
                } 

                var user = await _dbContext.User.FirstOrDefaultAsync(user => user.Username == userDto.Username);
                if(user is null) return null;

                if(new PasswordHasher<User>().VerifyHashedPassword(user, user.HashPassword, userDto.Password) == PasswordVerificationResult.Failed)
                {
                   return null;
                }

                TokenResponseDto reponse = new(
                    CreateToken(user), 
                    await GenerateAndSaveRefreshTokenAsync(user)
                    );
                    
                return reponse;
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

        private  string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role,user.Role)
            }; 

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token") ?? "")); 

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"), 
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1), 
                signingCredentials:creds 
            ); 

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32]; 
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken(); 
            user.RefreshToken = refreshToken; 
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 
            await UpdateAsync(user);
            return refreshToken;
        }
    }
}