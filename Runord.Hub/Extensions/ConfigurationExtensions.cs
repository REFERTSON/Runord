using Microsoft.Extensions.Options;
using Minio;
using Runord.Hub.Configs;
using Runord.Hub.Services;
using Runord.Hub.Services.Interfaces;

namespace Runord.Hub.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddAppConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            // Секции конфигурации
            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
            services.Configure<MinioSettings>(configuration.GetSection("Minio"));

            // Брокер очередей RabbitMQ
            services.AddSingleton<IQueueService, QueueService>();

            // Клиент объектного хранилища MinIO
            services.AddSingleton<IMinioClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
                return new MinioClient()
                    .WithEndpoint(settings.Endpoint)
                    .WithCredentials(settings.AccessKey, settings.SecretKey)
                    .WithSSL(settings.UseSsl)
                    .Build();
            });

            return services;
        }
    }
}
