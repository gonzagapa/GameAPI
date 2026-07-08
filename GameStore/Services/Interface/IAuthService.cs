using GameStore.Dtos;
using GameStore.Models;

namespace GameStore.Services.Interface
{
    public interface IAuthService
    {
        Task<string> LoginAsync(UserDto userDto);

        Task<User?> RegisterAsync(UserDto userDto);
    }
}