using Microsoft.AspNetCore.Authorization;

namespace Runord.Hub.Hubs
{
    [Authorize]
    public class ProjectHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SubscribeToProject(Guid projectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"project_{projectId}");
        }

        public async Task UnsubscribeFromProject(Guid projectId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project_{projectId}");
        }
    }
}