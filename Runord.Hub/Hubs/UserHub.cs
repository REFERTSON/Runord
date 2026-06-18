using Microsoft.AspNetCore.Authorization;

namespace Runord.Hub.Hubs
{
    [Authorize]
    public class UsersHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SubscribeToUsersUpdates()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "users_updates");
        }

        public async Task UnsubscribeFromUsersUpdates()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "users_updates");
        }
    }
}