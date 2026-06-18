using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Runord.Client.ViewModels.Base;
using System.IO;
using System.Windows.Media.Imaging;

namespace Runord.Client.ViewModels
{
    public partial class UserProfileViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _fullName = "Иван Иванов";

        [ObservableProperty]
        private string _email = "ivan.ivanov@example.com";

        [ObservableProperty]
        private string _role = "Administrator";

        [ObservableProperty]
        private BitmapImage? _avatar;

        [RelayCommand]
        private void ChangeAvatar()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Выберите аватар"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    Avatar = new BitmapImage(new Uri(dialog.FileName));
                }
                catch { }
            }
        }

        [RelayCommand]
        private void SaveProfile()
        {
            System.Windows.MessageBox.Show("Профиль сохранён", "Успех", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}