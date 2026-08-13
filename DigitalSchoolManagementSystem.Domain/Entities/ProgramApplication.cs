using DigitalSchoolManagementSystem.Domain.Common;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // One student's application to one program. Unique on (StudentId, ProgramId) - see OnModelCreating.
    public class ProgramApplication : BaseEntity
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int ProgramId { get; set; }
        public EducationProgram Program { get; set; } = null!;

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime? ReviewedAt { get; set; }
        public int? ReviewedByStaffId { get; set; }
        public User? ReviewedByStaff { get; set; }
        public string? ReviewNotes { get; set; }
    }
}
