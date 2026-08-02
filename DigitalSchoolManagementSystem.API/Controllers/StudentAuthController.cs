using DigitalSchoolManagementSystem.Application.DTOs.Auth;
using DigitalSchoolManagementSystem.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSchoolManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/auth/student")]
    public class StudentAuthController : ControllerBase
    {
        private readonly IStudentAuthService _studentAuthService;

        public StudentAuthController(IStudentAuthService studentAuthService)
        {
            _studentAuthService = studentAuthService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterStudentRequestDto request)
        {
            try
            {
                var result = await _studentAuthService.RegisterAsync(request);
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
                var result = await _studentAuthService.LoginAsync(request);
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
                var result = await _studentAuthService.RefreshTokenAsync(request);
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
                await _studentAuthService.RevokeTokenAsync(request);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
