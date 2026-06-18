using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Runord.Client.Data.Interfaces;
using Runord.Client.Services;
using Runord.Client.ViewModels.Base;
using Runord.Shared.DTOs.Notification;
using Runord.Shared.Enums;
using Runord.Shared.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Runord.Client.ViewModels
{

    public partial class NotificationCenterViewModel : ViewModelBase
    {
        private readonly INotificationService _notificationService;
        private readonly NotificationHubService _notificationHub;
        private readonly IUserSessionStorage _sessionStorage;

        public ObservableCollection<NotificationItemViewModel> Notifications { get; }
        public ICollectionView GroupedNotifications { get; }

        [ObservableProperty] private bool _isListEmpty;
        [ObservableProperty] private string _bannerHeader = "Нет новых уведомлений  (︶▽︶)";
        [ObservableProperty] private string _bannerMessage = "Ваш центр уведомлений абсолютно чист. Здесь будут появляться системные сообщения, статусы задач и предупреждения кластеров.";

        public NotificationCenterViewModel(INotificationService notificationService,
            NotificationHubService notificationHub, IUserSessionStorage sessionStorage)
        {
            _notificationService = notificationService;
            _notificationHub = notificationHub;
            _sessionStorage = sessionStorage;
            Notifications = new ObservableCollection<NotificationItemViewModel>();
            GroupedNotifications = CollectionViewSource.GetDefaultView(Notifications);
            GroupedNotifications.GroupDescriptions.Add(new PropertyGroupDescription(nameof(NotificationItemViewModel.DateGroup)));
            GroupedNotifications.SortDescriptions.Add(new SortDescription(nameof(NotificationItemViewModel.Timestamp), ListSortDirection.Descending));

            _notificationHub.NotificationReceived += OnNotificationReceived;
            LoadNotifications();
        }

        private async void LoadNotifications()
        {
            await ExecuteAsync(async () =>
            {
                var session = await _sessionStorage.GetCurrentSessionAsync();
                if (session == null) return;
                var result = await _notificationService.GetNotificationsAsync(session.UserId, null, CancellationToken.None);
                if (result.IsSuccess && result.Data != null)
                {
                    Notifications.Clear();
                    foreach (var dto in result.Data)
                        Notifications.Add(new NotificationItemViewModel(dto));
                    IsListEmpty = Notifications.Count == 0;
                }
            });
        }

        private void OnNotificationReceived(NotificationDto dto)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Notifications.Insert(0, new NotificationItemViewModel(dto));
                IsListEmpty = Notifications.Count == 0;
            });
        }

        [RelayCommand]
        private async Task MarkAsRead(NotificationItemViewModel item)
        {
            if (item == null) return;
            var session = await _sessionStorage.GetCurrentSessionAsync();
            if (session == null) return;
            await _notificationService.MarkAsReadAsync(session.UserId, item.Id, CancellationToken.None);
            item.IsRead = true;
        }

        [RelayCommand]
        private async Task DeleteNotification(NotificationItemViewModel item)
        {
            if (item == null) return;
            var session = await _sessionStorage.GetCurrentSessionAsync();
            if (session == null) return;
            await _notificationService.DeleteNotificationAsync(session.UserId, item.Id, CancellationToken.None);
            Notifications.Remove(item);
            IsListEmpty = Notifications.Count == 0;
        }

        [RelayCommand]
        private async Task ClearAll()
        {
            var session = await _sessionStorage.GetCurrentSessionAsync();
            if (session == null) return;
            await _notificationService.DeleteAllUserNotificationsAsync(session.UserId, CancellationToken.None);
            Notifications.Clear();
            IsListEmpty = true;
        }
    }

    public partial class NotificationItemViewModel : ObservableObject
    {
        public Guid Id { get; set; }
        [ObservableProperty] private DateTime _timestamp;
        [ObservableProperty] private NotificationType _type;
        [ObservableProperty] private string _title = "";
        [ObservableProperty] private string _message = "";
        [ObservableProperty] private bool _isRead;

        public string DateGroup
        {
            get
            {
                var date = Timestamp.Date;
                var today = DateTime.Today;
                if (date == today) return "Сегодня";
                if (date == today.AddDays(-1)) return "Вчера";
                if (date >= today.AddDays(-7)) return "На этой неделе";
                return "Ранее";
            }
        }

        public NotificationItemViewModel() { }
        public NotificationItemViewModel(NotificationDto dto)
        {
            Id = dto.Id;
            Timestamp = dto.CreatedAt.LocalDateTime;
            Type = dto.Type;
            Title = dto.Title;
            Message = dto.Message;
            IsRead = dto.IsRead;
        }
    }
}