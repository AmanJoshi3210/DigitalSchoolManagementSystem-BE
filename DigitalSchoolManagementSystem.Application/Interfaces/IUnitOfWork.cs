using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IStudentRepository Students { get; }
        IStaffUserRepository StaffUsers { get; }
        IGenericRepository<Role> Roles { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        ISubjectRepository Subjects { get; }
        IAttendanceRepository Attendances { get; }
        IExamRepository Exams { get; }
        IGenericRepository<ExamSubject> ExamSubjects { get; }
        IExamResultRepository ExamResults { get; }

        Task<int> SaveChangesAsync();
    }
}
