namespace GameStore.Dtos
{
    public record UserRegisterDto(
        string Username,
        DateTime CreatedAt
    );
}