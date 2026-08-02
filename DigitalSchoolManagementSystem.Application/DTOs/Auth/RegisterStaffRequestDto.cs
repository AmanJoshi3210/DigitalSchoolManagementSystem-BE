using System.ComponentModel.DataAnnotations;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.DTOs.Auth
{
    public class RegisterStaffRequestDto
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
        public string EmployeeCode { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Designation { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Department { get; set; }

        [Required]
        public DateTime JoiningDate { get; set; }

        [MaxLength(150)]
        public string? Qualification { get; set; }
        public decimal? Salary { get; set; }
    }
}
