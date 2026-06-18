using Runord.Cluster.Server;
using Runord.Cluster.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<MetricsAggregator>();
builder.Services.AddHostedService<HubReporterWorker>();

var app = builder.Build();

app.MapGrpcService<HealthReportingService>();

app.Run();