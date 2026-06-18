namespace Runord.Cluster.Server
{
    public class HubReporterWorker : BackgroundService
    {
        private readonly MetricsAggregator _aggregator;
        private readonly ILogger<HubReporterWorker> _logger;

        public HubReporterWorker(MetricsAggregator aggregator, ILogger<HubReporterWorker> logger)
        {
            _aggregator = aggregator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var (avgCpu, avgMemory) = _aggregator.GetClusterAverages();

                _logger.LogInformation("Агрегированные данные кластера: CPU {Cpu:F2}%, RAM {Ram:F2}%", avgCpu, avgMemory);

                // TODO: Вызов клиента для передачи данных в Hub (например, через SignalR, gRPC или RabbitMQ)
                // await _hubClient.SendClusterStatsAsync(avgCpu, avgMemory, stoppingToken);

                await Task.Delay(10000, stoppingToken); // Отчитываемся Хабу каждые 10 секунд
            }
        }
    }
}
