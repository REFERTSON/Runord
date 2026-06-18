using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Runord.Client.Data.Interfaces;
using Runord.Client.UIElements.Controls;
using Runord.Client.ViewModels.Base;
using Runord.Shared.Interfaces.Services;

namespace Runord.Client.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAuthService _authService;
        private readonly ITokenStorage _tokenStorage;
        private readonly IUserSessionStorage _sessionStorage;
        private readonly INavigationService _navigationService;

        [ObservableProperty] private ViewModelBase? _currentViewModel;

        public MainViewModel(IServiceProvider serviceProvider, IAuthService authService,
            ITokenStorage tokenStorage, IUserSessionStorage sessionStorage, INavigationService navigationService)
        {
            _serviceProvider = serviceProvider;
            _authService = authService;
            _tokenStorage = tokenStorage;
            _sessionStorage = sessionStorage;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task Navigate(NavigationType targetType)
        {
            ViewModelBase? vm = targetType switch
            {
                NavigationType.Dashboard => _serviceProvider.GetService<DashboardViewModel>(),
                NavigationType.TaskBuilder => _serviceProvider.GetService<TaskBuilderViewModel>(),
                NavigationType.ProjectsList => _serviceProvider.GetService<ProjectsListViewModel>(),
                NavigationType.TaskDispatcher => _serviceProvider.GetService<TaskDispatherViewModel>(),
                NavigationType.ClusterList => _serviceProvider.GetService<ClusterListViewModel>(),
                NavigationType.UserManagement => _serviceProvider.GetService<UserManagementViewModel>(),
                NavigationType.NotificationCenter => _serviceProvider.GetService<NotificationCenterViewModel>(),
                NavigationType.Settings => _serviceProvider.GetService<SettingsViewModel>(),
                NavigationType.UserProfile => _serviceProvider.GetService<UserProfileViewModel>(),
                NavigationType.TaskDetails => _serviceProvider.GetService<TaskDetailsViewModel>(),
                _ => null
            };
            if (vm != null)
                CurrentViewModel = vm;
        }

        [RelayCommand]
        private async Task LoadSession()
        {
            await ExecuteAsync(async () =>
            {
                var session = await _sessionStorage.GetCurrentSessionAsync();
                var accessToken = await _tokenStorage.GetAccessTokenAsync();
                if (session != null && !string.IsNullOrEmpty(accessToken))
                {
                    var refreshToken = await _tokenStorage.GetRefreshTokenAsync();
                    var refreshResult = await _authService.RefreshTokenAsync(refreshToken ?? "", CancellationToken.None);
                    if (refreshResult.IsSuccess)
                    {
                        await Navigate(NavigationType.Dashboard);
                    }
                    else
                    {
                        _navigationService.NavigateToLogin();
                    }
                }
                else
                {
                    _navigationService.NavigateToLogin();
                }
            });
        }

        [RelayCommand]
        private async Task Logout()
        {
            var session = await _sessionStorage.GetCurrentSessionAsync();
            if (session != null)
                await _authService.LogoutAsync(session.UserId, CancellationToken.None);
            _navigationService.NavigateToLogin();
        }
    }

    public interface INavigationService
    {
        void NavigateToLogin();
        void NavigateToMain();
    }
}