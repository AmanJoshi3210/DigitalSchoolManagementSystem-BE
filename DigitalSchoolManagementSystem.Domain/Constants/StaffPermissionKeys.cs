namespace DigitalSchoolManagementSystem.Domain.Constants
{
    // Grantable staff-portal page permissions. StaffRole.Admin always implicitly has all of
    // these (see RequireStaffPermissionAttribute) and never gets rows in StaffPermission.
    public static class StaffPermissionKeys
    {
        public const string ActionHub = "ActionHub";
        public const string Students = "Students";
        public const string Programs = "Programs";
        public const string Messages = "Messages";

        public static readonly IReadOnlyList<string> All = [ActionHub, Students, Programs, Messages];
    }
}
