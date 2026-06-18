using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Runord.Client.ViewModels.Base;
using Runord.Shared.Interfaces.Services;

namespace Runord.Client.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty] private string _email = "";
        [ObservableProperty] private string _password = "";
        [ObservableProperty] private bool _isLoggingIn;
        [ObservableProperty] private string _errorMessage = "";

        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Введите email и пароль";
                return;
            }

            IsLoggingIn = true;
            ErrorMessage = "";
            await ExecuteAsync(async () =>
            {
                var result = await _authService.LoginAsync(Email, Password, CancellationToken.None);
                if (result.IsSuccess)
                {
                    _navigationService.NavigateToMain();
                }
                else
                {
                    ErrorMessage = result.ErrorMessage ?? "Ошибка входа";
                }
            });
            IsLoggingIn = false;
        }
    }
}