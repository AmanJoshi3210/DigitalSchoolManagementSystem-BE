using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class ProgramApplicationRepository : GenericRepository<ProgramApplication>, IProgramApplicationRepository
    {
        public ProgramApplicationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ProgramApplication?> GetByIdWithDetailsAsync(int id) =>
            await DbSet.Include(a => a.Student).ThenInclude(s => s.User)
                .Include(a => a.Program)
                .Include(a => a.ReviewedByStaff)
                .SingleOrDefaultAsync(a => a.Id == id);

        public async Task<IReadOnlyList<ProgramApplication>> GetByProgramIdAsync(int programId) =>
            await DbSet.Include(a => a.Student).ThenInclude(s => s.User)
                .Include(a => a.Program)
                .Include(a => a.ReviewedByStaff)
                .Where(a => a.ProgramId == programId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task<IReadOnlyList<ProgramApplication>> GetByStudentIdAsync(int studentId) =>
            await DbSet.Include(a => a.Program)
                .Include(a => a.ReviewedByStaff)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task<bool> ExistsForStudentAndProgramAsync(int studentId, int programId) =>
            await DbSet.AnyAsync(a => a.StudentId == studentId && a.ProgramId == programId);
    }
}
