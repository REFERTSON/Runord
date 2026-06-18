using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Runord.Hub.Hubs
{
    [Authorize]
    public class NotificationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SubscribeToNotifications()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}_notifications");
        }

        public async Task UnsubscribeFromNotifications()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}_notifications");
        }
    }
}