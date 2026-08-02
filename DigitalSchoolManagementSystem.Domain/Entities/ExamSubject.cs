using DigitalSchoolManagementSystem.Domain.Common;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // Schedules one subject within an exam, with the marks scale for that paper.
    public class ExamSubject : BaseEntity
    {
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public DateTime ExamDate { get; set; }
        public decimal MaxMarks { get; set; }
        public decimal PassingMarks { get; set; }
    }
}
