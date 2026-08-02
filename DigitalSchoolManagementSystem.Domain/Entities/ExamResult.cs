using DigitalSchoolManagementSystem.Domain.Common;

namespace DigitalSchoolManagementSystem.Domain.Entities
{
    // A student's marks for one scheduled exam subject paper.
    public class ExamResult : BaseEntity
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int ExamSubjectId { get; set; }
        public ExamSubject ExamSubject { get; set; } = null!;

        public decimal MarksObtained { get; set; }
        public string? Grade { get; set; }
        public string? Remarks { get; set; }
    }
}
