using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IJwtTokenService
    {
        (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);
        (string Token, DateTime ExpiresAt) GenerateRefreshToken();
    }
}
