using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<Message?> GetLastMessageAsync(int conversationId);
        Task<IReadOnlyList<Message>> GetPageAsync(int conversationId, int page, int pageSize);
        Task<int> CountUnreadAsync(int conversationId, int userId, DateTime? since);
    }
}
