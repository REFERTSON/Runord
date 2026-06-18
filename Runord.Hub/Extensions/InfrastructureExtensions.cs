using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio;
using Runord.Hub.BackgroundServices;
using Runord.Hub.Configs;
using Runord.Hub.Data;
using Runord.Hub.Data.Repositories;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Hub.Services;
using Runord.Hub.Services.Interfaces;
using Runord.Shared.Interfaces;
using Runord.Shared.Interfaces.Services;

namespace Runord.Hub.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Кэширование в памяти
            services.AddMemoryCache();

            // База данных PostgreSQL
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Настройки MinIO
            services.Configure<MinioSettings>(configuration.GetSection("Minio"));
            services.AddSingleton<IMinioClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
                return new MinioClient()
                    .WithEndpoint(settings.Endpoint)
                    .WithCredentials(settings.AccessKey, settings.SecretKey)
                    .WithSSL(settings.UseSsl)
                    .Build();
            });

            // Настройки RabbitMQ
            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));

            // Репозитории
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskFileRepository, TaskFileRepository>();
            services.AddScoped<IClusterRepository, ClusterRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            // Сервисы
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddSingleton<IQueueService, QueueService>();
            services.AddScoped<IClusterSelector, ClusterSelector>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IUserService, UserService>();

            // SignalR и gRPC
            services.AddSignalR();
            services.AddGrpc();

            // Фоновые службы (Consumer'ы очередей)
            services.AddHostedService<TaskQueueConsumer>();
            services.AddHostedService<TaskResultConsumer>();
            services.AddHostedService<TokenCleanupWorker>();

            // CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            services.AddControllers();
            return services;
        }
    }
}