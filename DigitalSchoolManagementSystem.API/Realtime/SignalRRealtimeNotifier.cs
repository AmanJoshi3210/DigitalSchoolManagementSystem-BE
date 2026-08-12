using DigitalSchoolManagementSystem.API.Hubs;
using DigitalSchoolManagementSystem.Application.DTOs.Messaging;
using DigitalSchoolManagementSystem.Application.DTOs.Notifications;
using DigitalSchoolManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DigitalSchoolManagementSystem.API.Realtime
{
    public class SignalRRealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRRealtimeNotifier(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyNewMessageAsync(int recipientUserId, MessageDto message) =>
            _hubContext.Clients.Group(NotificationHub.GroupName(recipientUserId)).SendAsync("ReceiveMessage", message);

        public Task NotifyNewNotificationAsync(int recipientUserId, NotificationDto notification) =>
            _hubContext.Clients.Group(NotificationHub.GroupName(recipientUserId)).SendAsync("ReceiveNotification", notification);

        public Task NotifyConversationUpdatedAsync(int recipientUserId, ConversationDto conversation) =>
            _hubContext.Clients.Group(NotificationHub.GroupName(recipientUserId)).SendAsync("ConversationUpdated", conversation);
    }
}
