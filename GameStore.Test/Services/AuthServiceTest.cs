using GameStore.Data;
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
                ["AppSettings:Token"] = "test-signing-key-that-is-long-enough-1234567890",
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

            var secondAttempt = service.RegisterAsync8DT
        }


    }
}