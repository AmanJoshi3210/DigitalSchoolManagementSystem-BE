using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using DigitalSchoolManagementSystem.Infrastructure.Repositories;

namespace DigitalSchoolManagementSystem.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IUserRepository? _users;
        private IStudentRepository? _students;
        private IStaffUserRepository? _staffUsers;
        private IGenericRepository<Role>? _roles;
        private IRefreshTokenRepository? _refreshTokens;
        private ISubjectRepository? _subjects;
        private IAttendanceRepository? _attendances;
        private IExamRepository? _exams;
        private IGenericRepository<ExamSubject>? _examSubjects;
        private IExamResultRepository? _examResults;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IStudentRepository Students => _students ??= new StudentRepository(_context);
        public IStaffUserRepository StaffUsers => _staffUsers ??= new StaffUserRepository(_context);
        public IGenericRepository<Role> Roles => _roles ??= new GenericRepository<Role>(_context);
        public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);
        public ISubjectRepository Subjects => _subjects ??= new SubjectRepository(_context);
        public IAttendanceRepository Attendances => _attendances ??= new AttendanceRepository(_context);
        public IExamRepository Exams => _exams ??= new ExamRepository(_context);
        public IGenericRepository<ExamSubject> ExamSubjects => _examSubjects ??= new GenericRepository<ExamSubject>(_context);
        public IExamResultRepository ExamResults => _examResults ??= new ExamResultRepository(_context);

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
