using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.DTOs.Students
{
    public class StudentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? ProfileImageUrl { get; set; }

        public string AdmissionNumber { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public DateTime AdmissionDate { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianPhoneNumber { get; set; }
        public string? BloodGroup { get; set; }

        public bool IsActive { get; set; }
    }
}
