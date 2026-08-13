using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class EducationProgramRepository : GenericRepository<EducationProgram>, IEducationProgramRepository
    {
        public EducationProgramRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<EducationProgram>> GetActiveAsync() =>
            await DbSet.Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

        public async Task<IReadOnlyList<EducationProgram>> GetAllOrderedAsync() =>
            await DbSet.OrderBy(p => p.Name).ToListAsync();
    }
}
