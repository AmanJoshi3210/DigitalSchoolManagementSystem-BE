using DigitalSchoolManagementSystem.Application.DTOs.Notifications;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.IServices
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateAsync(int recipientUserId, NotificationType type, string title, string body, int? conversationId = null);
        Task<IReadOnlyList<NotificationDto>> GetMyNotificationsAsync(int userId, bool unreadOnly, int page, int pageSize);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int notificationId, int userId);
        Task MarkAllAsReadAsync(int userId);
    }
}
