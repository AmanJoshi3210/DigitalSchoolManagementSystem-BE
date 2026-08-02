using System.ComponentModel.DataAnnotations;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.DTOs.Auth
{
    public class RegisterStudentRequestDto
    {
        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }

        [Required, MaxLength(50)]
        public string AdmissionNumber { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string RollNumber { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Grade { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Section { get; set; } = string.Empty;

        [Required]
        public DateTime AdmissionDate { get; set; }

        public string? GuardianName { get; set; }
        public string? GuardianPhoneNumber { get; set; }
        public string? BloodGroup { get; set; }
    }
}
