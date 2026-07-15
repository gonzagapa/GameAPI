namespace GameStore.Dtos
{
    public record RefreshTokenDto(
         int UserId, 
         string RefreshToken 
    );
}