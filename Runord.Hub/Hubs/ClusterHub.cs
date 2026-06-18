using Microsoft.AspNetCore.Authorization;

namespace Runord.Hub.Hubs
{
    [Authorize(Roles = "Admin")]
    public class ClusterHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SubscribeToClusterUpdates()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "all_clusters");
        }

        public async Task UnsubscribeToClusterUpdates() { 
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "all_clusters");
        }
    }
}
