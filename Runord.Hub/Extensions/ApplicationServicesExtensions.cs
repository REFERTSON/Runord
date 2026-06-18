using Runord.Hub.BackgroundServices;
using Runord.Hub.Services;
using Runord.Hub.Services.Interfaces;
using Runord.Shared.Interfaces;
using Runord.Shared.Interfaces.Services;

namespace Runord.Hub.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClusterService, ClusterService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IClusterSelector, ClusterSelector>();

            return services;
        }
    }
}
