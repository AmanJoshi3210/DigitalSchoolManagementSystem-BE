using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class AnnouncementRepository : GenericRepository<Announcement>, IAnnouncementRepository
    {
        public AnnouncementRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Announcement>> GetByTargetGradeAsync(string targetGrade) =>
            await DbSet.Where(a => a.TargetGrade == targetGrade && a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
    }
}
