using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Runord.Client.Data;
using Runord.Client.Data.Interfaces;
using Runord.Client.Data.Repositories;
using Runord.Client.Handlers;
using Runord.Client.Services;
using Runord.Client.ViewModels;
using Runord.Shared.Interfaces;
using Runord.Shared.Interfaces.Services;
using System.IO;
using System.Windows;

namespace Runord.Client
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            InitializeDatabase();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // SQLite
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Runord", "appdata.db");
            var folder = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder!);
            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Storage
            services.AddSingleton<ITokenStorage, TokenStorage>();
            services.AddSingleton<IUserSessionStorage, UserSessionStorage>();

            // HTTP Client
            services.AddTransient<AuthenticatedHttpClientHandler>();
            services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001/"); // замените на ваш URL
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClusterService, ClusterService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISettingsService, SettingsService>();

            // SignalR
            services.AddSingleton<NotificationHubService>();

            // Navigation
            services.AddSingleton<INavigationService, NavigationService>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<ClusterListViewModel>();
            services.AddTransient<ProjectsListViewModel>();
            services.AddTransient<NotificationCenterViewModel>();
            services.AddTransient<TaskDispatherViewModel>();
            services.AddTransient<UserManagementViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<UserProfileViewModel>();
            services.AddTransient<TaskBuilderViewModel>();
            services.AddTransient<TaskDetailsViewModel>();

            // Windows
            services.AddSingleton<MainWindow>();
        }

        private void InitializeDatabase()
        {
            using var scope = ServiceProvider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using var context = factory.CreateDbContext();
            context.Database.EnsureCreated();
        }
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        public NavigationService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public void NavigateToLogin()
        {
            var loginWindow = new LoginView();
            loginWindow.Show();
            Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
        }

        public void NavigateToMain()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
        }
    }
}