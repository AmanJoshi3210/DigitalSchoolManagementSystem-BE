using DigitalSchoolManagementSystem.Domain.Entities;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IReadOnlyList<Notification>> GetForUserAsync(int userId, bool unreadOnly, int page, int pageSize);
        Task<int> GetUnreadCountAsync(int userId);
        Task<IReadOnlyList<Notification>> GetUnreadForUserAsync(int userId);
    }
}
