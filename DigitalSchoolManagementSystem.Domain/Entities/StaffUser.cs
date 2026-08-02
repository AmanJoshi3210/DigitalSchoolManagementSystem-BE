using DigitalSchoolManagementSystem.Domain.Common;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // Staff-specific details (teachers, admin staff, etc). Shared identity/login fields live on the linked User.
    public class StaffUser : BaseEntity
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string? Department { get; set; }
        public DateTime JoiningDate { get; set; }
        public string? Qualification { get; set; }
        public decimal? Salary { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
