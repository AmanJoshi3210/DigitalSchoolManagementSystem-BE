using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Domain.Enums;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Conversation?> GetByIdWithParticipantsAsync(int id) =>
            await DbSet
                .Include(c => c.Participants).ThenInclude(p => p.User).ThenInclude(u => u.Role)
                .Include(c => c.CreatedByUser)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IReadOnlyList<Conversation>> GetForUserAsync(int userId) =>
            await DbSet
                .Include(c => c.Participants).ThenInclude(p => p.User).ThenInclude(u => u.Role)
                .Where(c => c.IsActive && c.Participants.Any(p => p.UserId == userId && p.IsActive))
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .ToListAsync();

        public async Task<Conversation?> FindDirectConversationAsync(int userId1, int userId2) =>
            await DbSet
                .Include(c => c.Participants).ThenInclude(p => p.User).ThenInclude(u => u.Role)
                .Where(c => c.Type == ConversationType.Direct && c.IsActive
                    && c.Participants.Count(p => p.IsActive) == 2
                    && c.Participants.Any(p => p.UserId == userId1 && p.IsActive)
                    && c.Participants.Any(p => p.UserId == userId2 && p.IsActive))
                .FirstOrDefaultAsync();
    }
}
