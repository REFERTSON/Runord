using Runord.Shared.DTOs.Task;

namespace Runord.Hub.Services.Interfaces
{
    public interface IQueueService
    {
        Task<bool> PublishTaskAsync(TaskQueueMessage message, CancellationToken cancellationToken = default);
        void SubscribeTaskQueue(Func<TaskQueueMessage, CancellationToken, Task> onMessage);
        void SubscribeResultQueue(Func<TaskResultMessage, CancellationToken, Task> onMessage);
    }
}