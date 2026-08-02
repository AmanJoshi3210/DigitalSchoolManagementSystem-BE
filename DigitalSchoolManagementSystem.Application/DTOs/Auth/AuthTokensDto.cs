namespace DigitalSchoolManagementSystem.Application.DTOs.Auth
{
    public record AuthTokensDto(
        string AccessToken,
        DateTime AccessTokenExpiresAt,
        string RefreshToken,
        DateTime RefreshTokenExpiresAt);
}
