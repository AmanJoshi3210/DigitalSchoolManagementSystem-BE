using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Message?> GetLastMessageAsync(int conversationId) =>
            await DbSet
                .Where(m => m.ConversationId == conversationId && m.IsActive)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();

        public async Task<IReadOnlyList<Message>> GetPageAsync(int conversationId, int page, int pageSize)
        {
            var messages = await DbSet
                .Include(m => m.Sender).ThenInclude(u => u.Role)
                .Where(m => m.ConversationId == conversationId && m.IsActive)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            messages.Reverse();
            return messages;
        }

        public async Task<int> CountUnreadAsync(int conversationId, int userId, DateTime? since) =>
            await DbSet.CountAsync(m => m.ConversationId == conversationId && m.IsActive
                && m.SenderUserId != userId
                && (since == null || m.CreatedAt > since));
    }
}
