using System.ComponentModel.DataAnnotations;

namespace DigitalSchoolManagementSystem.Application.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
