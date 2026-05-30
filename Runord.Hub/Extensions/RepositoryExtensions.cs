using Runord.Hub.Data.Repositories;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IClusterRepository, ClusterRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskFileRepository, TaskFileRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Единица работы (Unit of Work)
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            return services;
        }
    }
}
