using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IAttendanceRepository : IGenericRepository<Attendance>
    {
        Task<Attendance?> GetByStudentAndDateAsync(int studentId, DateTime date);
        Task<IReadOnlyList<Attendance>> GetByStudentAsync(int studentId, DateTime? from, DateTime? to);
    }
}
