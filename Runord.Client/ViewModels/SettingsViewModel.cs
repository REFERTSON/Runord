using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Runord.Client.ViewModels.Base;
using System.Collections.ObjectModel;

namespace Runord.Client.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _darkThemeEnabled = true;

        [ObservableProperty]
        private bool _notificationsEnabled = true;

        [ObservableProperty]
        private string _apiEndpoint = "https://localhost:5001";

        [ObservableProperty]
        private int _refreshInterval = 30;

        public ObservableCollection<string> Languages { get; } = new() { "Русский", "English" };
        [ObservableProperty]
        private string _selectedLanguage = "Русский";

        [RelayCommand]
        private void SaveSettings()
        {
            // Здесь будет сохранение настроек
            System.Windows.MessageBox.Show("Настройки сохранены", "Успех", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ResetToDefault()
        {
            DarkThemeEnabled = true;
            NotificationsEnabled = true;
            ApiEndpoint = "https://localhost:5001";
            RefreshInterval = 30;
            SelectedLanguage = "Русский";
        }
    }
}