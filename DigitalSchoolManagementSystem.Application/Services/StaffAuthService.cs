using DigitalSchoolManagementSystem.Application.DTOs.Auth;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class StaffAuthService : IStaffAuthService
    {
        private const string RoleName = "Staff";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthTokenService _authTokenService;

        public StaffAuthService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IAuthTokenService authTokenService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _authTokenService = authTokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterStaffRequestDto request)
        {
            if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
                throw new InvalidOperationException("Email is already registered.");

            if (await _unitOfWork.Users.UsernameExistsAsync(request.Username))
                throw new InvalidOperationException("Username is already taken.");

            if (await _unitOfWork.StaffUsers.ExistsAsync(s => s.EmployeeCode == request.EmployeeCode))
                throw new InvalidOperationException("Employee code is already in use.");

            var role = await GetOrCreateRoleAsync();

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Role = role
            };

            var staffUser = new StaffUser
            {
                EmployeeCode = request.EmployeeCode,
                Designation = request.Designation,
                Department = request.Department,
                JoiningDate = request.JoiningDate,
                Qualification = request.Qualification,
                Salary = request.Salary,
                User = user
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.StaffUsers.AddAsync(staffUser);
            await _unitOfWork.SaveChangesAsync();

            var tokens = await _authTokenService.IssueTokensAsync(user);
            return BuildAuthResponse(user, tokens);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByUsernameOrEmailWithDetailsAsync(request.UsernameOrEmail);

            if (user is null || user.StaffUser is null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials.");

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            var tokens = await _authTokenService.IssueTokensAsync(user);
            return BuildAuthResponse(user, tokens);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var (tokens, user) = await _authTokenService.RefreshTokensAsync(request.RefreshToken);

            if (user.StaffUser is null)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            return BuildAuthResponse(user, tokens);
        }

        public Task RevokeTokenAsync(RefreshTokenRequestDto request) =>
            _authTokenService.RevokeTokenAsync(request.RefreshToken);

        private async Task<Role> GetOrCreateRoleAsync()
        {
            var role = await _unitOfWork.Roles.SingleOrDefaultAsync(r => r.Name == RoleName);
            if (role is not null)
                return role;

            role = new Role { Name = RoleName };
            await _unitOfWork.Roles.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();
            return role;
        }

        private static AuthResponseDto BuildAuthResponse(User user, AuthTokensDto tokens) => new()
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.Name,
            AccessToken = tokens.AccessToken,
            AccessTokenExpiresAt = tokens.AccessTokenExpiresAt,
            RefreshToken = tokens.RefreshToken,
            RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt
        };
    }
}
