using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Runord.Hub.Services.Interfaces;
using Runord.Shared.Entities;

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
            // var clusterExecutor = scope.ServiceProvider.GetRequiredService<IClusterExecutor>(); // gRPC клиент для вызова кластера

            queueService.SubscribeTaskQueue(async (message, cancellationToken) =>
            {
                try
                {
                    _logger.LogInformation("Получена задача {TaskId} типа {TaskType}", message.TaskId, message.TaskType);

                    // Определение требований к ресурсам (заглушка)
                    double requiredCpu = 1.0;
                    double requiredRam = 1.0;

                    var selectedCluster = await clusterSelector.SelectClusterAsync(message.TaskType, requiredCpu, requiredRam, cancellationToken);
                    if (selectedCluster == null)
                    {
                        _logger.LogWarning("Нет доступных кластеров для задачи {TaskId}. Повторная попытка позже.", message.TaskId);
                        throw new Exception("No cluster available"); // Сообщение не будет подтверждено – вернётся в очередь
                    }

                    _logger.LogInformation("Задача {TaskId} назначена кластеру {ClusterName}", message.TaskId, selectedCluster.Name);

                    // Отправка задачи на кластер через gRPC (реализуйте IClusterExecutor)
                    // await clusterExecutor.ExecuteTaskAsync(selectedCluster, message, cancellationToken);

                    // После успешной отправки сообщение будет подтверждено автоматически (в SubscribeTaskQueue)
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка обработки задачи {TaskId}", message.TaskId);
                    // Сообщение НЕ подтверждается, вернётся в очередь
                    throw;
                }
            });

            while (!stoppingToken.IsCancellationRequested)
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}