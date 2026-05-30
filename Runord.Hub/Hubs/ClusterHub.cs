using Runord.Shared.DTOs.Cluster;
using Microsoft.AspNetCore.SignalR;

namespace Runord.Hub.Hubs
{
    public class ClusterHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SubscribeToClusterUpdates()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "all_clusters");
        }

        // Метод вызывается из фонового сервиса при обновлении метрик
        public async Task SendMetricsUpdate(ClusterDto dto)
        {
            await Clients.Group("all_clusters").SendAsync("MetricsUpdated", dto);
        }
    }
}
