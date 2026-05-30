using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Runord.Client.UIElements.Models;
using Runord.Client.UIElements.Models.Sidebar;
using System.Collections.ObjectModel;

namespace Runord.Client.UIElements.ViewModels
{
    public partial class SidebarViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isExpanded;

        [ObservableProperty]
        private SidebarUserProfile _currentUser;

        public ObservableCollection<SidebarNavigationItem> MainMenu { get; } = new();
        public ObservableCollection<SidebarNavigationItem> SystemMenu { get; } = new();
        public ObservableCollection<SidebarNavigationItem> BottomMenu { get; } = new();

        public SidebarViewModel()
        {
            // Инициализация профиля пользователя
            CurrentUser = new SidebarUserProfile
            {
                Name = "Иван Петров",
                Role = "Администратор",
                Icon_path = "pack://application:,,,/Runord.Client.UIElements;component/Assets/avatar.png" // Замените на реальный путь
            };

            LoadMockData();
        }

        [RelayCommand]
        private void ToggleMenu()
        {
            IsExpanded = !IsExpanded;
        }

        [RelayCommand]
        private void SelectItem(SidebarNavigationItem selectedItem)
        {
            if (selectedItem == null) return;

            // Сбрасываем выделение у всех элементов
            var allItems = MainMenu.Concat(SystemMenu).Concat(BottomMenu);
            foreach (var item in allItems)
            {
                item.IsSelected = false;
            }

            selectedItem.IsSelected = true;
            selectedItem.ExecuteAction?.Invoke();
        }

        private void LoadMockData()
        {
            // Пример данных (иконки можно заменить на реальные Geometry из вашего проекта)
            MainMenu.Add(new SidebarNavigationItem { Title = "Главная", IconData = "M10,20V14H14V20H19V12H22L12,3L2,12H5V20H10Z", IsSelected = true });
            MainMenu.Add(new SidebarNavigationItem { Title = "Конструктор", IconData = "M21,16.5C21,16.88 ... " });
            MainMenu.Add(new SidebarNavigationItem { Title = "Проекты", IconData = "M3,3H21V21H3V3M5,5V19H19V5H5Z" });

            SystemMenu.Add(new SidebarNavigationItem { Title = "Диспетчер", IconData = "M16,11V15H8V11H16M19,15H21V19H3V15H5V11H19V15Z" });
            SystemMenu.Add(new SidebarNavigationItem { Title = "Кластеры", IconData = "M4,4H10V10H4V4M14,4H20V10H14V4Z" });
            SystemMenu.Add(new SidebarNavigationItem { Title = "Пользователи", IconData = "M12,4A4,4 0 0,1 16,8A4,4 0 0,1 12,12A4,4 0 0,1 8,8A4,4 0 0,1 12,4Z" });
            SystemMenu.Add(new SidebarNavigationItem { Title = "Терминал", IconData = "M20,19V7H4V19H20M20,3A2,2 0 0,1 22,5V19A2,2 0 0,1 20,21H4A2,2 0 0,1 2,19V5A2,2 0 0,1 4,3H20M13,17V15H18V17H13M9.58,13L5.57,9H8.4L12.41,13L8.4,17H5.57L9.58,13Z" });

            BottomMenu.Add(new SidebarNavigationItem { Title = "Уведомления", IconData = "M12,22A2,2 0 0,0 14,20H10A2,2 0 0,0 12,22M18,16V11C18,7.93 16.36,5.36 13.5,4.68V4A1.5,1.5 0 0,0 12,2.5A1.5,1.5 0 0,0 10.5,4V4.68C7.63,5.36 6,7.92 6,11V16L4,18V19H20V18L18,16Z" });
            BottomMenu.Add(new SidebarNavigationItem { Title = "Настройки", IconData = "M12,15.5A3.5,3.5 0 0,1 8.5,12A3.5,3.5 0 0,1 12,8.5A3.5,3.5 0 0,1 15.5,12A3.5,3.5 0 0,1 12,15.5M19.43,12.98C19.47,12.66 19.5,12.34 19.5,12C19.5,11.66 19.47,11.34 19.43,11L21.54,9.37C21.73,9.22 21.78,8.95 21.66,8.73L19.66,5.27C19.54,5.05 19.27,4.96 19.05,5.05L16.56,6.05C16.04,5.65 15.48,5.32 14.87,5.07L14.5,2.42C14.46,2.18 14.25,2 14,2H10C9.75,2 9.54,2.18 9.5,2.42L9.13,5.07C8.52,5.32 7.96,5.66 7.44,6.05L4.95,5.05C4.73,4.96 4.46,5.05 4.34,5.27L2.34,8.73C2.21,8.95 2.27,9.22 2.46,9.37L4.57,11C4.53,11.34 4.5,11.67 4.5,12C4.5,12.33 4.53,12.65 4.57,12.98L2.46,14.63C2.27,14.78 2.21,15.05 2.34,15.27L4.34,18.73C4.46,18.95 4.73,19.03 4.95,18.95L7.44,17.94C7.96,18.34 8.52,18.68 9.13,18.93L9.5,21.58C9.54,21.82 9.75,22 10,22H14C14.25,22 14.46,21.82 14.5,21.58L14.87,18.93C15.48,18.68 16.04,18.34 16.56,17.94L19.05,18.95C19.27,19.03 19.54,18.95 19.66,18.73L21.66,15.27C21.78,15.05 21.73,14.78 21.54,14.63L19.43,12.98Z" });
        }
    }
}
