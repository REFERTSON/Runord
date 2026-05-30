using Microsoft.AspNetCore.SignalR;
using Runord.Shared.DTOs.Task;

namespace Runord.Hub.Hubs
{
    public class ProjectHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SubscribeToProject(Guid projectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"project_{projectId}");
        }

        // Теперь TaskShortDto содержит ProjectId
        public async Task TaskUpdated(TaskDto task)
        {
            await Clients.Group($"project_{task.ProjectId}").SendAsync("TaskUpdated", task);
        }
    }
}