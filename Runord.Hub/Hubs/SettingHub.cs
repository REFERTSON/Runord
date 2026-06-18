using Microsoft.AspNetCore.Authorization;

namespace Runord.Hub.Hubs
{
    [Authorize]
    public class SettingHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SubscribeToSettingsUpdates()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "settings_updates");
        }

        public async Task UnsubscribeFromSettingsUpdates()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "settings_updates");
        }
    }
}