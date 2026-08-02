using DigitalSchoolManagementSystem.Application.DTOs.Auth;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthTokenService(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthTokensDto> IssueTokensAsync(User user)
        {
            var (accessToken, accessExpiresAt) = _jwtTokenService.GenerateAccessToken(user);
            var (refreshToken, refreshExpiresAt) = _jwtTokenService.GenerateRefreshToken();

            await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = refreshExpiresAt,
                UserId = user.Id
            });
            await _unitOfWork.SaveChangesAsync();

            return new AuthTokensDto(accessToken, accessExpiresAt, refreshToken, refreshExpiresAt);
        }

        public async Task<(AuthTokensDto Tokens, User User)> RefreshTokensAsync(string refreshToken)
        {
            var existing = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            if (!existing.IsValid)
                throw new UnauthorizedAccessException("Refresh token is expired or has already been used.");

            var (accessToken, accessExpiresAt) = _jwtTokenService.GenerateAccessToken(existing.User);
            var (newRefreshToken, refreshExpiresAt) = _jwtTokenService.GenerateRefreshToken();

            existing.RevokedAt = DateTime.UtcNow;
            existing.ReplacedByToken = newRefreshToken;
            _unitOfWork.RefreshTokens.Update(existing);

            await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresAt = refreshExpiresAt,
                UserId = existing.UserId
            });
            await _unitOfWork.SaveChangesAsync();

            return (new AuthTokensDto(accessToken, accessExpiresAt, newRefreshToken, refreshExpiresAt), existing.User);
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var existing = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            if (!existing.IsValid)
                return;

            existing.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(existing);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
