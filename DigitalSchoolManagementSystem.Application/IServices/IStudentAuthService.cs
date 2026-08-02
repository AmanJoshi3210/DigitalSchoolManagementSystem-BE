using DigitalSchoolManagementSystem.Application.DTOs.Auth;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IStudentAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterStudentRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task RevokeTokenAsync(RefreshTokenRequestDto request);
    }
}
