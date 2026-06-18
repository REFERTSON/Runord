using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Runord.Client.UIElements.Controls
{
    public enum NavigationType
    {
        Dashboard,
        ProjectsList,
        TaskBuilder,
        TaskDispatcher,
        ClusterList,
        UserManagement,
        NotificationCenter,
        Settings,
        UserProfile,
        ProjectDetails,
        TaskDetails
    }

    public partial class SidebarMenu : UserControl
    {
        public event EventHandler<NavigationType>? NavigateRequested;

        public static readonly DependencyProperty AvatarSourceProperty =
            DependencyProperty.Register(
                nameof(AvatarSource),
                typeof(ImageSource),
                typeof(SidebarMenu),
                new PropertyMetadata(null));

        public ImageSource? AvatarSource
        {
            get => (ImageSource?)GetValue(AvatarSourceProperty);
            set => SetValue(AvatarSourceProperty, value);
        }

        // Свойство для контроля видимости красной полоски на карточке профиля
        public static readonly DependencyProperty IsProfileActiveProperty =
            DependencyProperty.Register(
                nameof(IsProfileActive),
                typeof(bool),
                typeof(SidebarMenu),
                new PropertyMetadata(false, OnIsProfileActiveChanged));

        public bool IsProfileActive
        {
            get => (bool)GetValue(IsProfileActiveProperty);
            set => SetValue(IsProfileActiveProperty, value);
        }

        public SidebarMenu()
        {
            InitializeComponent();
        }

        private static void OnIsProfileActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SidebarMenu sidebar && (bool)e.NewValue)
            {
                // Если профиль стал активен — сбрасываем выделение со всех кнопок меню
                sidebar.ClearAllMenuButtonsSelection();
            }
        }

        private void MenuButton_Checked(object sender, RoutedEventArgs e)
        {
            // БЕЗОПАСНО ГАСИМ ПОЛОСКУ НА ПРОФИЛЕ: так как сработало событие Checked у кнопок меню
            IsProfileActive = false;

            // Надежно ищем кастомную кнопку SidebarMenuButton вверх по визуальному дереву
            DependencyObject current = e.OriginalSource as DependencyObject;
            while (current != null && !(current is SidebarMenuButton))
            {
                current = VisualTreeHelper.GetParent(current);
            }

            // Если нашли кнопку — вызываем событие навигации
            if (current is SidebarMenuButton menuButton)
            {
                if (menuButton.Tag is NavigationType navType)
                {
                    NavigateRequested?.Invoke(this, navType);
                }
            }
        }

        public void OpenUserProfile()
        {
            IsProfileActive = true;
            NavigateRequested?.Invoke(this, NavigationType.UserProfile);
        }

        private void ProfileMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            IsProfileActive = true;
            NavigateRequested?.Invoke(this, NavigationType.UserProfile);
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigateRequested?.Invoke(this, NavigationType.Settings);
        }

        // Рекурсивный поиск и сброс активного состояния (IsActive) у кнопок в меню
        private void ClearAllMenuButtonsSelection()
        {
            ClearSelectionInContainer(Menu);
        }

        private void ClearSelectionInContainer(DependencyObject container)
        {
            if (container == null) return;

            int count = VisualTreeHelper.GetChildrenCount(container);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(container, i);
                if (child is SidebarMenuButton btn)
                {
                    btn.IsActive = false;
                }
                else
                {
                    ClearSelectionInContainer(child);
                }
            }
        }
    }
}