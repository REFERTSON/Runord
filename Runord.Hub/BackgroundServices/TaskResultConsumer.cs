using Microsoft.AspNetCore.SignalR;
using Runord.Hub.Hubs;
using Runord.Hub.Services.Interfaces;
using Runord.Shared.Interfaces.Services;

namespace Runord.Hub.BackgroundServices
{
    public class TaskResultConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TaskResultConsumer> _logger;

        public TaskResultConsumer(IServiceProvider serviceProvider, ILogger<TaskResultConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
            var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();
            var fileStorage = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<TaskHub>>();

            queueService.SubscribeResultQueue(async (resultMessage, cancellationToken) =>
            {
                try
                {
                    _logger.LogInformation("Получен результат задачи {TaskId}. Успех: {IsSuccess}", resultMessage.TaskId, resultMessage.IsSuccess);

                    var newStatus = resultMessage.IsSuccess ? Shared.Enums.TaskStatus.Completed : Shared.Enums.TaskStatus.Failed;

                    // Обновляем статус в БД
                    await taskService.UpdateTaskStatusAsync(resultMessage.TaskId, newStatus, null, true, cancellationToken);

                    // Если есть выходные файлы – сохраняем в MinIO (они уже там, просто создаём записи)
                    // (здесь можно дописать логику сохранения метаданных файлов)

                    // Уведомляем клиента через SignalR
                    await hubContext.Clients.Group($"task_{resultMessage.TaskId}").SendAsync("TaskResult", resultMessage);
                    _logger.LogInformation("Уведомление отправлено клиенту для задачи {TaskId}", resultMessage.TaskId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка обработки результата задачи {TaskId}", resultMessage.TaskId);
                }
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}