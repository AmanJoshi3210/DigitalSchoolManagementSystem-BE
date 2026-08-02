using DigitalSchoolManagementSystem.Application.DTOs.Academics;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface IAttendanceService
    {
        Task<AttendanceDto> MarkAsync(MarkAttendanceDto request);
        Task<IReadOnlyList<AttendanceDto>> GetByStudentAsync(int studentId, DateTime? from, DateTime? to);
        Task<AttendanceSummaryDto> GetSummaryAsync(int studentId, DateTime? from, DateTime? to);
    }
}
