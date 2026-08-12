using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DigitalSchoolManagementSystem.API.Hubs
{
    // Push-only hub: clients connect and receive ReceiveMessage / ReceiveNotification / ConversationUpdated
    // events. All writes go through the REST endpoints, not through hub methods.
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(Context.UserIdentifier!));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(Context.UserIdentifier!));
            await base.OnDisconnectedAsync(exception);
        }

        public static string GroupName(int userId) => GroupName(userId.ToString());

        public static string GroupName(string userId) => $"user:{userId}";
    }
}
