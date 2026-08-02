using DigitalSchoolManagementSystem.Application.DTOs.Auth;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IStaffAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterStaffRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task RevokeTokenAsync(RefreshTokenRequestDto request);
    }
}
