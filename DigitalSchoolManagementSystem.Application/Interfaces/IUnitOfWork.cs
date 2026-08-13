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
        IAnnouncementRepository Announcements { get; }
        IConversationRepository Conversations { get; }
        IGenericRepository<ConversationParticipant> ConversationParticipants { get; }
        IMessageRepository Messages { get; }
        INotificationRepository Notifications { get; }
        IEducationProgramRepository Programs { get; }
        IProgramApplicationRepository ProgramApplications { get; }

        Task<int> SaveChangesAsync();
    }
}
