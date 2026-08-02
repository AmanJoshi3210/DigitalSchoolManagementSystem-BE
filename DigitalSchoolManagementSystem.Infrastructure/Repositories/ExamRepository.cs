using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class ExamRepository : GenericRepository<Exam>, IExamRepository
    {
        public ExamRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Exam?> GetByIdWithSubjectsAsync(int id) =>
            await DbSet.Include(e => e.ExamSubjects).ThenInclude(es => es.Subject)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IReadOnlyList<Exam>> SearchAsync(string? grade, string? academicYear)
        {
            var query = DbSet.Where(e => e.IsActive);

            if (!string.IsNullOrWhiteSpace(grade))
                query = query.Where(e => e.Grade == grade);

            if (!string.IsNullOrWhiteSpace(academicYear))
                query = query.Where(e => e.AcademicYear == academicYear);

            return await query.OrderByDescending(e => e.StartDate).ToListAsync();
        }

        public async Task<ExamSubject?> GetExamSubjectAsync(int examSubjectId) =>
            await Context.Set<ExamSubject>()
                .Include(es => es.Exam)
                .Include(es => es.Subject)
                .SingleOrDefaultAsync(es => es.Id == examSubjectId);

        public async Task<bool> SubjectAlreadyScheduledAsync(int examId, int subjectId) =>
            await Context.Set<ExamSubject>().AnyAsync(es => es.ExamId == examId && es.SubjectId == subjectId);
    }
}
