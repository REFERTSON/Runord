using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Runord.Client.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Runord.Client.ViewModels
{
    public partial class UserManagementViewModel : ViewModelBase
    {
        public ObservableCollection<UserItemViewModel> Users { get; set; }

        public UserManagementViewModel()
        {
            Users = new ObservableCollection<UserItemViewModel>();
            Users.CollectionChanged += OnUsersChanged;

            // Заполняем тестовыми данными для проверки отображения сетки
            Users.Add(new UserItemViewModel { UserName = "Системный Администратор", UserRole = "Super Admin", Email = "admin@runord.sys", LastActivity = "Сейчас", IsOnline = true });
            Users.Add(new UserItemViewModel { UserName = "Алексей Иванов", UserRole = "DevOps", Email = "a.ivanov@runord.sys", LastActivity = "10 мин. назад", IsOnline = true });
            Users.Add(new UserItemViewModel { UserName = "Мария Петрова", UserRole = "Developer", Email = "m.petrova@runord.sys", LastActivity = "Вчера 18:30", IsOnline = false });
            Users.Add(new UserItemViewModel { UserName = "Сергей Сидоров", UserRole = "QA Engineer", Email = "s.sidorov@runord.sys", LastActivity = "Сейчас", IsOnline = true });
            Users.Add(new UserItemViewModel { UserName = "Анна Смирнова", UserRole = "Project Manager", Email = "a.smirnova@runord.sys", LastActivity = "2 часа назад", IsOnline = false });
            Users.Add(new UserItemViewModel { UserName = "Дмитрий Волков", UserRole = "Security", Email = "d.volkov@runord.sys", LastActivity = "Сейчас", IsOnline = true });
            Users.Add(new UserItemViewModel { UserName = "Екатерина Лебедева", UserRole = "Developer", Email = "e.lebedeva@runord.sys", LastActivity = "3 дня назад", IsOnline = false });
            Users.Add(new UserItemViewModel { UserName = "Иван Николаев", UserRole = "Data Analyst", Email = "i.nikolaev@runord.sys", LastActivity = "15 мин. назад", IsOnline = true });
            Users.Add(new UserItemViewModel { UserName = "Ольга Соколова", UserRole = "HR", Email = "o.sokolova@runord.sys", LastActivity = "Сегодня 09:15", IsOnline = false });
            Users.Add(new UserItemViewModel { UserName = "Павел Морозов", UserRole = "SysAdmin", Email = "p.morozov@runord.sys", LastActivity = "Сейчас", IsOnline = true });
            Users.Add(new UserItemViewModel { UserName = "Елена Кузнецова", UserRole = "Viewer", Email = "e.kuznetsova@runord.sys", LastActivity = "Неделю назад", IsOnline = false });
            Users.Add(new UserItemViewModel { UserName = "Михаил Зайцев", UserRole = "Developer", Email = "m.zaitsev@runord.sys", LastActivity = "Сейчас", IsOnline = true });
            Users.Add(new UserItemViewModel { UserName = "Виктория Новикова", UserRole = "Designer", Email = "v.novikova@runord.sys", LastActivity = "Вчера 12:00", IsOnline = false });
            Users.Add(new UserItemViewModel { UserName = "Роман Григорьев", UserRole = "DevOps", Email = "r.grigorev@runord.sys", LastActivity = "5 мин. назад", IsOnline = true });
            Users.Add(new UserItemViewModel { UserName = "Татьяна Орлова", UserRole = "Product Owner", Email = "t.orlova@runord.sys", LastActivity = "2 часа назад", IsOnline = false });
        }

        // Динамические счетчики
        public int TotalCount => Users.Count;
        public int OnlineCount => Users.Count(x => x.IsOnline);
        public int OfflineCount => Users.Count(x => !x.IsOnline);
        public bool IsListEmpty => Users.Count == 0;

        // Обработка добавления/удаления пользователей в коллекции
        private void OnUsersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (UserItemViewModel user in e.NewItems)
                {
                    user.PropertyChanged += OnUserPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (UserItemViewModel user in e.OldItems)
                {
                    user.PropertyChanged -= OnUserPropertyChanged;
                }
            }

            RefreshCounters();
        }

        // Обработка изменения свойств конкретного пользователя (например, смена статуса на онлайн)
        private void OnUserPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserItemViewModel.IsOnline))
            {
                RefreshCounters();
            }
        }

        // Обновление UI для счетчиков
        private void RefreshCounters()
        {
            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(OnlineCount));
            OnPropertyChanged(nameof(OfflineCount));
            OnPropertyChanged(nameof(IsListEmpty));
        }

        // Команда удаления
        [RelayCommand]
        private void DeleteUser(UserItemViewModel user)
        {
            if (user != null && Users.Contains(user))
            {
                Users.Remove(user);
            }
        }

        // Команда редактирования
        [RelayCommand]
        private void EditUser(UserItemViewModel user)
        {
            // Логика редактирования пользователя (например, открытие модального окна)
        }
    }

    public partial class UserItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _userName;

        [ObservableProperty]
        private string _userRole;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _lastActivity;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusText))] // Автоматически обновляет StatusText при изменении IsOnline
        private bool _isOnline;

        // Статус текст вычисляется на лету
        public string StatusText => IsOnline ? "В сети" : "Не в сети";
    }
}