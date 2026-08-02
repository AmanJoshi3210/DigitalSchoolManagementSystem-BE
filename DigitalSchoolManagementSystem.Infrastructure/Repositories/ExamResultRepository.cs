using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class ExamResultRepository : GenericRepository<ExamResult>, IExamResultRepository
    {
        public ExamResultRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ExamResult?> GetByStudentAndExamSubjectAsync(int studentId, int examSubjectId) =>
            await DbSet
                .Include(r => r.ExamSubject).ThenInclude(es => es.Subject)
                .Include(r => r.ExamSubject).ThenInclude(es => es.Exam)
                .SingleOrDefaultAsync(r => r.StudentId == studentId && r.ExamSubjectId == examSubjectId);

        public async Task<IReadOnlyList<ExamResult>> GetByStudentAndExamAsync(int studentId, int examId) =>
            await DbSet
                .Include(r => r.ExamSubject).ThenInclude(es => es.Subject)
                .Include(r => r.ExamSubject).ThenInclude(es => es.Exam)
                .Where(r => r.StudentId == studentId && r.ExamSubject.ExamId == examId)
                .ToListAsync();

        public async Task<IReadOnlyList<ExamResult>> GetByStudentAsync(int studentId) =>
            await DbSet
                .Include(r => r.ExamSubject).ThenInclude(es => es.Subject)
                .Include(r => r.ExamSubject).ThenInclude(es => es.Exam)
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.ExamSubject.ExamDate)
                .ToListAsync();
    }
}
