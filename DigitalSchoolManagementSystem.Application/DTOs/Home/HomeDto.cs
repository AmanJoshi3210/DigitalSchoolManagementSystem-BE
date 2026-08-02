namespace DigitalSchoolManagementSystem.Application.DTOs.Home
{
    public class HomeDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string Greeting { get; set; } = string.Empty;

        public StudentHomeInfoDto? StudentInfo { get; set; }
        public StaffHomeInfoDto? StaffInfo { get; set; }
    }

    public class StudentHomeInfoDto
    {
        public int StudentId { get; set; }
        public string AdmissionNumber { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public decimal AttendancePercentage { get; set; }
        public int UpcomingExamsCount { get; set; }
    }

    public class StaffHomeInfoDto
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string? Department { get; set; }
        public int TotalActiveStudents { get; set; }
    }
}
