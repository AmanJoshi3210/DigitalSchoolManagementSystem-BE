namespace DigitalSchoolManagementSystem.Application.DTOs.Academics
{
    public class ExamResultDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ExamSubjectId { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public decimal MarksObtained { get; set; }
        public decimal MaxMarks { get; set; }
        public decimal PassingMarks { get; set; }
        public string? Grade { get; set; }
        public bool Passed { get; set; }
        public string? Remarks { get; set; }
    }

    public class RecordExamResultDto
    {
        public int StudentId { get; set; }
        public int ExamSubjectId { get; set; }
        public decimal MarksObtained { get; set; }
        public string? Remarks { get; set; }
    }
}
