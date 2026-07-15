namespace GameStore.Dtos
{
    public record TokenResponseDto(
        string AccessToken,
        string RefreshToken
    );
}