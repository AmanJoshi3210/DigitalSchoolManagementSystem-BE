using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.DTOs.Students
{
    public class UpdateStudentDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? ProfileImageUrl { get; set; }

        public string Grade { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string? GuardianName { get; set; }
        public string? GuardianPhoneNumber { get; set; }
        public string? BloodGroup { get; set; }
    }
}
