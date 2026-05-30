namespace Runord.Hub.Hubs
{
    public class NotificationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SubscribeToNotifications(string userId)
            => await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}_notifications");

        public async Task UnsubscribeFromNotifications(string userId)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}_notifications");
    }
}