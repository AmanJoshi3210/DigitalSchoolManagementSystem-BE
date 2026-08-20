using DigitalSchoolManagementSystem.Domain.Common;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // Staff-specific details (teachers, admin staff, etc). Shared identity/login fields live on the linked User.
    public class StaffUser : BaseEntity
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public StaffRole Role { get; set; }
        public string? Department { get; set; }
        public DateTime JoiningDate { get; set; }
        public string? Qualification { get; set; }
        public decimal? Salary { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<StaffPermission> Permissions { get; set; } = new List<StaffPermission>();
    }
}
