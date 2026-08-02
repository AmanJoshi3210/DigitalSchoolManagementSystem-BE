using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.DTOs.Students
{
    // Fields a student may edit on their own profile. Grade, section, roll number and
    // admission details are staff-controlled and updated via the staff Update endpoint.
    public class UpdateOwnProfileDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
