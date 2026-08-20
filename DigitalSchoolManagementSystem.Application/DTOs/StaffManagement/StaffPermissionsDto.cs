namespace DigitalSchoolManagementSystem.Application.DTOs.StaffManagement
{
    public class StaffPermissionsDto
    {
        public int StaffUserId { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
