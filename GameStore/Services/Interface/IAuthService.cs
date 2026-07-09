using GameStore.Dtos;
using GameStore.Models;

namespace GameStore.Services.Interface
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> LoginAsync(UserDto userDto);

        Task<User?> RegisterAsync(UserDto userDto);

        Task<string?> LogoutAsync(int userId);

        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenDto request);
    }
}