using DigitalSchoolManagementSystem.Domain.Common;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // Announcement posted by a staff member, targeted at students of a specific grade.
    public class Announcement : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string TargetGrade { get; set; } = string.Empty;

        public int StaffId { get; set; }
        public StaffUser Staff { get; set; } = null!;
        public string StaffName { get; set; } = string.Empty;
    }
}
