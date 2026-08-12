using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Notification>> GetForUserAsync(int userId, bool unreadOnly, int page, int pageSize)
        {
            var query = DbSet.Where(n => n.RecipientUserId == userId && n.IsActive);
            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId) =>
            await DbSet.CountAsync(n => n.RecipientUserId == userId && n.IsActive && !n.IsRead);

        public async Task<IReadOnlyList<Notification>> GetUnreadForUserAsync(int userId) =>
            await DbSet.Where(n => n.RecipientUserId == userId && n.IsActive && !n.IsRead).ToListAsync();
    }
}
