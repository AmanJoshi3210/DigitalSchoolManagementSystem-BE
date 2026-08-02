using DigitalSchoolManagementSystem.Domain.Common;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // Student-specific details. Shared identity/login fields live on the linked User.
    public class Student : BaseEntity
    {
        public string AdmissionNumber { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public DateTime AdmissionDate { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianPhoneNumber { get; set; }
        public string? BloodGroup { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
