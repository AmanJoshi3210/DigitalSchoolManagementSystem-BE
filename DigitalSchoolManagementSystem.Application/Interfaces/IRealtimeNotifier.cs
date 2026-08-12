using DigitalSchoolManagementSystem.Application.DTOs.Messaging;
using DigitalSchoolManagementSystem.Application.DTOs.Notifications;

namespace DigitalSchoolManagementSystem.Application.Interfaces
{
    // Push-transport abstraction (implemented with SignalR in the API layer) so Application
    // services stay unaware of the real-time transport in use.
    public interface IRealtimeNotifier
    {
        Task NotifyNewMessageAsync(int recipientUserId, MessageDto message);
        Task NotifyNewNotificationAsync(int recipientUserId, NotificationDto notification);
        Task NotifyConversationUpdatedAsync(int recipientUserId, ConversationDto conversation);
    }
}
