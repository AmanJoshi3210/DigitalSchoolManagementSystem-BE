using DigitalSchoolManagementSystem.Application.DTOs.Notifications;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Application.IServices;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Domain.Enums;

namespace DigitalSchoolManagementSystem.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRealtimeNotifier _realtimeNotifier;

        public NotificationService(IUnitOfWork unitOfWork, IRealtimeNotifier realtimeNotifier)
        {
            _unitOfWork = unitOfWork;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task<NotificationDto> CreateAsync(int recipientUserId, NotificationType type, string title, string body, int? conversationId = null)
        {
            var notification = new Notification
            {
                RecipientUserId = recipientUserId,
                Type = type,
                Title = title,
                Body = body,
                ConversationId = conversationId
            };
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            var dto = ToDto(notification);
            await _realtimeNotifier.NotifyNewNotificationAsync(recipientUserId, dto);
            return dto;
        }

        public async Task<IReadOnlyList<NotificationDto>> GetMyNotificationsAsync(int userId, bool unreadOnly, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;

            var notifications = await _unitOfWork.Notifications.GetForUserAsync(userId, unreadOnly, page, pageSize);
            return notifications.Select(ToDto).ToList();
        }

        public Task<int> GetUnreadCountAsync(int userId) => _unitOfWork.Notifications.GetUnreadCountAsync(userId);

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId)
                ?? throw new KeyNotFoundException("Notification not found.");

            if (notification.RecipientUserId != userId)
                throw new UnauthorizedAccessException("This notification does not belong to you.");

            if (notification.IsRead)
                return;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var unread = await _unitOfWork.Notifications.GetUnreadForUserAsync(userId);
            if (unread.Count == 0)
                return;

            var now = DateTime.UtcNow;
            foreach (var notification in unread)
            {
                notification.IsRead = true;
                notification.ReadAt = now;
                _unitOfWork.Notifications.Update(notification);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private static NotificationDto ToDto(Notification notification) => new()
        {
            Id = notification.Id,
            Type = notification.Type,
            Title = notification.Title,
            Body = notification.Body,
            ConversationId = notification.ConversationId,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            CreatedAt = notification.CreatedAt
        };
    }
}
