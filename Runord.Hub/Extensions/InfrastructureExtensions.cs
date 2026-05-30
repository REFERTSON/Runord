using Microsoft.EntityFrameworkCore;
using Runord.Hub.BackgroundServices;
using Runord.Hub.Data;

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

            // Настройка CORS для фронтенда
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Фоновые службы (Consumers)
            services.AddHostedService<TaskQueueConsumer>();
            services.AddHostedService<TaskResultConsumer>();

            // Системные UI компоненты
            services.AddControllers();
            services.AddSignalR();

            return services;
        }
    }
}
