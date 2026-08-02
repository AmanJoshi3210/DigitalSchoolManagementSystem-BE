namespace DigitalSchoolManagementSystem.Application.DTOs.Academics
{
    public class StudentAcademicsSummaryDto
    {
        public int StudentId { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public List<SubjectDto> Subjects { get; set; } = new();
        public decimal AttendancePercentage { get; set; }
        public List<ExamResultDto> RecentResults { get; set; } = new();
    }
}
