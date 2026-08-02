using DigitalSchoolManagementSystem.Application.DTOs.Auth;
using DigitalSchoolManagementSystem.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSchoolManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/auth/staff")]
    public class StaffAuthController : ControllerBase
    {
        private readonly IStaffAuthService _staffAuthService;

        public StaffAuthController(IStaffAuthService staffAuthService)
        {
            _staffAuthService = staffAuthService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterStaffRequestDto request)
        {
            try
            {
                var result = await _staffAuthService.RegisterAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
        {
            try
            {
                var result = await _staffAuthService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            try
            {
                var result = await _staffAuthService.RefreshTokenAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken(RefreshTokenRequestDto request)
        {
            try
            {
                await _staffAuthService.RevokeTokenAsync(request);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
