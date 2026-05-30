using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Runord.Hub.Services.Interfaces;
using Runord.Shared.DTOs.Task;
using Runord.Shared.Interfaces;

namespace Runord.Hub.BackgroundServices
{
    public class TaskQueueConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TaskQueueConsumer> _logger;

        public TaskQueueConsumer(IServiceProvider serviceProvider, ILogger<TaskQueueConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
            var clusterSelector = scope.ServiceProvider.GetRequiredService<IClusterSelector>();

            queueService.SubscribeTaskQueue(async (message, cancellationToken) =>
            {
                try
                {
                    _logger.LogInformation("Получена задача {TaskId} типа {TaskType}", message.TaskId, message.TaskType);

                    // Выбираем кластер (например, всегда 1 CPU и 1 GB RAM для простоты)
                    var requiredCpu = 1.0;
                    var requiredRam = 1.0;
                    var selectedCluster = await clusterSelector.SelectClusterAsync(message.TaskType, requiredCpu, requiredRam, cancellationToken);

                    if (selectedCluster == null)
                    {
                        _logger.LogWarning("Нет доступных кластеров для задачи {TaskId}. Задача остаётся в очереди.", message.TaskId);
                        return; // Не подтверждаем (reject) – RabbitMQ переотправит позже
                    }

                    _logger.LogInformation("Задача {TaskId} направлена на кластер {ClusterName}", message.TaskId, selectedCluster.Name);

                    // Здесь нужно отправить задачу на кластер (HTTP/gRPC)
                    // Например, вызов кластера через ClusterExecutor
                    // await _clusterExecutor.ExecuteTaskAsync(selectedCluster, message);

                    // После успешной отправки – подтверждаем (ack) сообщение уже сделано в SubscribeTaskQueue
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка обработки задачи {TaskId}", message.TaskId);
                    // Не подтверждаем – сообщение вернётся в очередь
                    throw;
                }
            });

            // Бесконечное ожидание (background service должен держать процесс живым)
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}