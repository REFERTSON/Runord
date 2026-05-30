using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Runord.Hub.Hubs
{
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

        public async Task RequestAvailableTaskTypes()
        {
            // Можно через сервис, но для простоты оставляем заглушку
            var types = new List<string> { "Матричное умножение", "Вычисление Pi" };
            await Clients.Caller.SendAsync("TaskTypesReceived", types);
        }
    }
}