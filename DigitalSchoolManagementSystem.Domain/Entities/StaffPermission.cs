using DigitalSchoolManagementSystem.Domain.Common;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // A single granted page permission for a staff user. Revoking a permission deletes the row
    // (no soft-delete) - the codebase has no global query filter, so a soft-deleted row would
    // silently still count as granted wherever it's read.
    public class StaffPermission : BaseEntity
    {
        public int StaffUserId { get; set; }
        public StaffUser StaffUser { get; set; } = null!;

        public string PermissionKey { get; set; } = string.Empty;
    }
}
