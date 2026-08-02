using DigitalSchoolManagementSystem.Application.DTOs.Academics;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class StudentAcademicsService : IStudentAcademicsService
    {
        private const int RecentResultsCount = 10;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubjectService _subjectService;
        private readonly IAttendanceService _attendanceService;
        private readonly IExamResultService _examResultService;

        public StudentAcademicsService(
            IUnitOfWork unitOfWork,
            ISubjectService subjectService,
            IAttendanceService attendanceService,
            IExamResultService examResultService)
        {
            _unitOfWork = unitOfWork;
            _subjectService = subjectService;
            _attendanceService = attendanceService;
            _examResultService = examResultService;
        }

        public async Task<StudentAcademicsSummaryDto?> GetSummaryAsync(int studentId)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId);
            if (student is null)
                return null;

            var subjects = await _subjectService.GetAllAsync(student.Grade);
            var attendanceSummary = await _attendanceService.GetSummaryAsync(studentId, null, null);
            var results = await _examResultService.GetByStudentAsync(studentId);

            return new StudentAcademicsSummaryDto
            {
                StudentId = studentId,
                Grade = student.Grade,
                Section = student.Section,
                Subjects = subjects.ToList(),
                AttendancePercentage = attendanceSummary.AttendancePercentage,
                RecentResults = results.Take(RecentResultsCount).ToList()
            };
        }
    }
}
