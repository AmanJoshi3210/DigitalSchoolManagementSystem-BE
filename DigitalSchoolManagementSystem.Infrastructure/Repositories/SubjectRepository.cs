using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Subject>> GetByGradeAsync(string grade) =>
            await DbSet.Where(s => s.Grade == grade && s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

        public async Task<bool> ExistsForGradeAsync(string grade, string code) =>
            await DbSet.AnyAsync(s => s.Grade == grade && s.Code == code);
    }
}
