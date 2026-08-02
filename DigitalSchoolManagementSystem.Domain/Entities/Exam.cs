using DigitalSchoolManagementSystem.Domain.Common;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // An examination window for a grade (e.g. "Mid Term" for Grade 10, 2025-2026).
    public class Exam : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<ExamSubject> ExamSubjects { get; set; } = new List<ExamSubject>();
    }
}
