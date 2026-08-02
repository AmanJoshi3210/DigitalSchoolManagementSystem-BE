using DigitalSchoolManagementSystem.Application.DTOs.Auth;
using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    // Shared token issuance/rotation used by both student and staff auth flows.
    public interface IAuthTokenService
    {
        Task<AuthTokensDto> IssueTokensAsync(User user);
        Task<(AuthTokensDto Tokens, User User)> RefreshTokensAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
    }
}
