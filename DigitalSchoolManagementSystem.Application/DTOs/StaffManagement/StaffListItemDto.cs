using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.DTOs.StaffManagement
{
    public class StaffListItemDto
    {
        public int StaffUserId { get; set; }
        public int UserId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public StaffRole StaffRole { get; set; }
        public string? Department { get; set; }
        public bool IsActive { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
