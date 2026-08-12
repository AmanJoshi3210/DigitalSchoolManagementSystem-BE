using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface IConversationRepository : IGenericRepository<Conversation>
    {
        Task<Conversation?> GetByIdWithParticipantsAsync(int id);
        Task<IReadOnlyList<Conversation>> GetForUserAsync(int userId);
        Task<Conversation?> FindDirectConversationAsync(int userId1, int userId2);
    }
}
