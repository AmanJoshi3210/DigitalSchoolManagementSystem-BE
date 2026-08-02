using DigitalSchoolManagementSystem.Application.DTOs.Home;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class HomeService : IHomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttendanceService _attendanceService;
        private readonly IExamService _examService;

        public HomeService(IUnitOfWork unitOfWork, IAttendanceService attendanceService, IExamService examService)
        {
            _unitOfWork = unitOfWork;
            _attendanceService = attendanceService;
            _examService = examService;
        }

        public async Task<HomeDto?> GetHomeAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetWithDetailsAsync(userId);
            if (user is null)
                return null;

            var home = new HomeDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role.Name,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                Greeting = $"Welcome back, {user.FirstName}!"
            };

            if (user.Student is not null)
            {
                var attendanceSummary = await _attendanceService.GetSummaryAsync(
                    user.Student.Id, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
                var upcomingExams = await _examService.GetAllAsync(user.Student.Grade, null);

                home.StudentInfo = new StudentHomeInfoDto
                {
                    StudentId = user.Student.Id,
                    AdmissionNumber = user.Student.AdmissionNumber,
                    Grade = user.Student.Grade,
                    Section = user.Student.Section,
                    RollNumber = user.Student.RollNumber,
                    AttendancePercentage = attendanceSummary.AttendancePercentage,
                    UpcomingExamsCount = upcomingExams.Count(e => e.EndDate >= DateTime.UtcNow)
                };
            }

            if (user.StaffUser is not null)
            {
                var activeStudents = await _unitOfWork.Students.CountAsync(s => s.IsActive);

                home.StaffInfo = new StaffHomeInfoDto
                {
                    EmployeeCode = user.StaffUser.EmployeeCode,
                    Designation = user.StaffUser.Designation,
                    Department = user.StaffUser.Department,
                    TotalActiveStudents = activeStudents
                };
            }

            return home;
        }
    }
}
