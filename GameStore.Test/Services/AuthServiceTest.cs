using GameStore.Data;
using GameStore.Data.Migrations;
using GameStore.Dtos;
using GameStore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GameStore.Test.Services
{
    public class AuthServiceTest
    {
        private static GameStoreContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<GameStoreContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            return new GameStoreContext(options);
        }

        private static IConfiguration CreateConfiguration()
        {
            var settings = new Dictionary<string, string?>
            {
                ["AppSettings:Token"] = "test-signing-key-that-is-long-enough-1234567890eVeryVeryLong!!!111oneeleven",
                ["AppSettings:Issuer"]= "TestIssuer",
                ["AppSettings:Audience"] = "TestAudience"
            }; 

            return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        }

        [Fact]
        public async Task RegisterAsync_should_create_user_with_hashed_password()
        {
            //Arrange
            using var context = CreateContext(); 
            var service = new AuthService(CreateConfiguration(),context); 
            var dto = new UserDto("gonzalo", "MyPassword123!");

            //act 
            var user = await service.RegisterAsync(dto); 
            Assert.NotNull(user); 
            Assert.Equal("gonzalo", user!.Username);
            Assert.NotEqual(dto.Password, user.HashPassword);
        }

        [Fact]
        public async Task RegisterAsync_should_return_null_when_username_already_exists()
        {
            using var context = CreateContext();
            var service = new AuthService(CreateConfiguration(),context); 
            var dto = new UserDto("gonzalo", "MyPassword123!");
            await service.RegisterAsync(dto); 

            var user = await service.RegisterAsync(dto);
            Assert.Null(user);
        }

        [Fact]
        public async Task LoginAsync_should_return_token_response_with_valid_credentials(){
            using var context = CreateContext(); 
            var service = new AuthService(CreateConfiguration(), context);
            var userDto = new UserDto("gonzalo", "MyPassword123!");
            await service.RegisterAsync(userDto);

            var response = await service.LoginAsync(userDto);  
            Assert.NotNull(response);
            Assert.NotEmpty(response.AccessToken);
            Assert.NotEmpty(response.RefreshToken);
            Assert.False(string.IsNullOrWhiteSpace(response.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(response.RefreshToken));
        } 

        [Theory]
        [InlineData("gonzalo", "WrongPassword123!")]
        [InlineData("juanes", "MyPassword123!")]
        public async Task LoginAsync_should_return_null_with_invalid_userDto(string username, string password)
        {
             using var context = CreateContext();
            var service = new AuthService(CreateConfiguration(), context);
            await service.RegisterAsync(new UserDto("gonzalo", "MyPassword123!"));

            var response = await service.LoginAsync(new UserDto(username, password));
            Assert.Null(response);
        }
        
        [Fact]
        public async Task Logout_should_return_string_response_with_valid_userId()
        {
            using var context = CreateContext(); 
            var service = new AuthService(CreateConfiguration(), context);
            await service.RegisterAsync(new UserDto("gonzalo", "MyPassword123!"));

            var response = await service.LogoutAsync(1);
            Assert.False(String.IsNullOrEmpty(response)); 
        }

        [Fact]
        public async Task Logout_should_return_null_with_invalid_userId()
        {
             using var context = CreateContext(); 
            var service = new AuthService(CreateConfiguration(), context);
            await service.RegisterAsync(new UserDto("gonzalo", "MyPassword123!"));

            var response = await service.LogoutAsync(3);
            Assert.Null(response); 
        }

        [Fact]
        public async Task Logout_should_remove_refreshToken_when_executed()
        {
            using var context = CreateContext(); 
            var service = new AuthService(CreateConfiguration(), context);
            var dto = new UserDto("gonzalo", "MyPassword123!");
            var user = await service.RegisterAsync(dto);

            await service.LoginAsync(dto);
            await service.LogoutAsync(1); 

            Assert.Null(user!.RefreshToken);
            Assert.Null(user!.RefreshTokenExpiryTime);
        }


    }
}