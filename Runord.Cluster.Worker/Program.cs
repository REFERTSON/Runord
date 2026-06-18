using Runord.Cluster.Worker;
using Runord.Cluster.Worker.Grpc;

var builder = Host.CreateApplicationBuilder(args);


// Подключаем кроссплатформенный мониторинг ресурсов
builder.Services.AddResourceMonitoring();

// Настраиваем gRPC клиент
builder.Services.AddGrpcClient<HealthReporter.HealthReporterClient>(o =>
{
    o.Address = new Uri("https://localhost:7164");
});

builder.Services.AddHostedService<SystemMetricsProvider>();

var host = builder.Build();
host.Run();