using GameStore.Models;
using GameStore.Dtos;

namespace GameStore.Mapping
{
    public static class UserMapping
    {
       public static UserRegisterDto MappingRegisterDto(this User user)
        {
            return new(user.Username, DateTime.UtcNow);
        } 
    }
}