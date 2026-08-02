namespace DigitalSchoolManagementSystem.Application.DTOs.Academics
{
    public class ExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ExamSubjectDto> Subjects { get; set; } = new();
    }

    public class CreateExamDto
    {
        public string Name { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class UpdateExamDto
    {
        public string Name { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ExamSubjectDto
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public decimal MaxMarks { get; set; }
        public decimal PassingMarks { get; set; }
    }

    public class AddExamSubjectDto
    {
        public int SubjectId { get; set; }
        public DateTime ExamDate { get; set; }
        public decimal MaxMarks { get; set; }
        public decimal PassingMarks { get; set; }
    }

    public class UpdateExamSubjectDto
    {
        public DateTime ExamDate { get; set; }
        public decimal MaxMarks { get; set; }
        public decimal PassingMarks { get; set; }
    }
}
