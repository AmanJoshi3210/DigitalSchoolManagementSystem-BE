using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token) =>
            await DbSet
                .Include(rt => rt.User).ThenInclude(u => u.Role)
                .Include(rt => rt.User).ThenInclude(u => u.Student)
                .Include(rt => rt.User).ThenInclude(u => u.StaffUser)
                .SingleOrDefaultAsync(rt => rt.Token == token);
    }
}
