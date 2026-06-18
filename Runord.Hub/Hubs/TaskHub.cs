using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Runord.Hub.Hubs
{
    [Authorize]
    public class TaskHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SubscribeToTask(string taskId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"task_{taskId}");
        }

        public async Task UnsubscribeFromTask(string taskId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"task_{taskId}");
        }

        // Клиент может сообщить о прогрессе задачи (опционально)
        public async Task SendProgress(Guid taskId, int percent)
        {
            await Clients.Group($"task_{taskId}").SendAsync("TaskProgress", taskId, percent);
        }
    }
}