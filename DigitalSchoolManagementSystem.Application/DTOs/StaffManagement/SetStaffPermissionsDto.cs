namespace DigitalSchoolManagementSystem.Application.DTOs.StaffManagement
{
    // The full desired permission set for a staff user - the server diffs this against current
    // rows (add what's missing, remove what's no longer requested), it's not an add/remove delta.
    public class SetStaffPermissionsDto
    {
        public List<string> Permissions { get; set; } = new();
    }
}
