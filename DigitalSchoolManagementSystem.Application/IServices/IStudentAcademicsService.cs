using DigitalSchoolManagementSystem.Application.DTOs.Academics;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IStudentAcademicsService
    {
        Task<StudentAcademicsSummaryDto?> GetSummaryAsync(int studentId);
    }
}
