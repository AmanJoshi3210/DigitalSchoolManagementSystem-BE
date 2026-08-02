using DigitalSchoolManagementSystem.Domain.Common;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // One attendance record per student per calendar date.
    public class Attendance : BaseEntity
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public DateTime Date { get; set; }
        public AttendanceStatus Status { get; set; }
        public string? Remarks { get; set; }
    }
}
