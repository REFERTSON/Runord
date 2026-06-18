using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Runord.Client.ViewModels.Base;
using System.Collections.ObjectModel;

namespace Runord.Client.ViewModels
{
    public partial class DashboardViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _userName = "REFERTSON";

        [ObservableProperty]
        private int _totalClustersCount = 9;

        [ObservableProperty]
        private int _activeTasksCount = 3;

        [ObservableProperty]
        private int _projectsCount = 3;

        public ObservableCollection<DashboardTaskViewModel> ActiveTasks { get; set; }
        public ObservableCollection<DashboardLogViewModel> SystemLogs { get; set; }
        public ObservableCollection<DashboardProjectViewModel> RecentProjects { get; set; }
        public ObservableCollection<DashboardClusterLoadViewModel> TopLoadedClusters { get; set; }

        public DashboardViewModel()
        {
            ActiveTasks = new ObservableCollection<DashboardTaskViewModel>();
            SystemLogs = new ObservableCollection<DashboardLogViewModel>();
            RecentProjects = new ObservableCollection<DashboardProjectViewModel>();
            TopLoadedClusters = new ObservableCollection<DashboardClusterLoadViewModel>();

            LoadMockData();
        }

        private void LoadMockData()
        {
            // 1. Активные задачи (Точно как на макете)
            ActiveTasks.Add(new DashboardTaskViewModel
            {
                Uuid = "4bf5rhnthj-rhrtyh...190...",
                Name = "Умножение матриц",
                Status = "Выполняется",
                Progress = 75
            });
            ActiveTasks.Add(new DashboardTaskViewModel
            {
                Uuid = "4bf5rhnthj-rhrtyh...490...",
                Name = "Вычисление Пи",
                Status = "Ожидание",
                Progress = 20
            });
            ActiveTasks.Add(new DashboardTaskViewModel
            {
                Uuid = "4bf5rhnthj-rhrtyh...490...",
                Name = "Вычисление Пи",
                Status = "Ожидание",
                Progress = 0
            });

            // 2. Журнал системы
            SystemLogs.Add(new DashboardLogViewModel
            {
                Timestamp = "26-06-2026 15:45",
                Level = "INFO",
                Message = "Задача: Умножение матриц 100х100 - Завершена"
            });
            SystemLogs.Add(new DashboardLogViewModel
            {
                Timestamp = "26-06-2026 15:44",
                Level = "WARNING",
                Message = "Зафиксирован дисбаланс вычислений <20%"
            });
            SystemLogs.Add(new DashboardLogViewModel
            {
                Timestamp = "26-06-2026 15:43",
                Level = "ERROR",
                Message = "Потеря связи с кластером: NODE-07"
            });
            SystemLogs.Add(new DashboardLogViewModel
            {
                Timestamp = "26-06-2026 15:45",
                Level = "INFO",
                Message = "Задача: Умножение матриц 100х100 - Завершена"
            });
            SystemLogs.Add(new DashboardLogViewModel
            {
                Timestamp = "26-06-2026 15:44",
                Level = "WARNING",
                Message = "Зафиксирован дисбаланс вычислений <20%"
            });
            SystemLogs.Add(new DashboardLogViewModel
            {
                Timestamp = "26-06-2026 15:43",
                Level = "ERROR",
                Message = "Потеря связи с кластером: NODE-07"
            });
            SystemLogs.Add(new DashboardLogViewModel
            {
                Timestamp = "26-06-2026 15:45",
                Level = "INFO",
                Message = "Задача: Умножение матриц 100х100 - Завершена"
            });
            SystemLogs.Add(new DashboardLogViewModel
            {
                Timestamp = "26-06-2026 15:44",
                Level = "WARNING",
                Message = "Зафиксирован дисбаланс вычислений <20%"
            });
            SystemLogs.Add(new DashboardLogViewModel
            {
                Timestamp = "26-06-2026 15:43",
                Level = "ERROR",
                Message = "Потеря связи с кластером: NODE-07"
            });
            SystemLogs.Add(new DashboardLogViewModel
            {
                Timestamp = "26-06-2026 15:45",
                Level = "INFO",
                Message = "Задача: Умножение матриц 100х100 - Завершена"
            });
            SystemLogs.Add(new DashboardLogViewModel
            {
                Timestamp = "26-06-2026 15:44",
                Level = "WARNING",
                Message = "Зафиксирован дисбаланс вычислений <20%"
            });
            SystemLogs.Add(new DashboardLogViewModel
            {
                Timestamp = "26-06-2026 15:43",
                Level = "ERROR",
                Message = "Потеря связи с кластером: NODE-07"
            });

            // 3. Недавние проекты
            RecentProjects.Add(new DashboardProjectViewModel { Title = "Умножение матриц", TimeAgo = "Изменение 3 минуты назад" });
            RecentProjects.Add(new DashboardProjectViewModel { Title = "Умножение матриц", TimeAgo = "Изменение 20 минут назад" });
            RecentProjects.Add(new DashboardProjectViewModel { Title = "Умножение матриц", TimeAgo = "Изменение 1 час назад" });

            // 4. Наиболее нагруженные кластеры
            TopLoadedClusters.Add(new DashboardClusterLoadViewModel { NodeName = "NODE-03", Ip = "192.168.1.10", LoadPercentage = 60 });
            TopLoadedClusters.Add(new DashboardClusterLoadViewModel { NodeName = "NODE-02", Ip = "192.168.1.15", LoadPercentage = 60 });
            TopLoadedClusters.Add(new DashboardClusterLoadViewModel { NodeName = "NODE-03", Ip = "192.168.1.18", LoadPercentage = 60 });
        }

        [RelayCommand]
        private void CancelTask(DashboardTaskViewModel task)
        {
            if (task != null && ActiveTasks.Contains(task))
            {
                ActiveTasks.Remove(task);
                ActiveTasksCount = ActiveTasks.Count;
            }
        }

        [RelayCommand]
        private void OpenAllTasks() { /* Логика */ }

        [RelayCommand]
        private void OpenFullLog() { /* Логика */ }

        [RelayCommand]
        private void OpenAllProjects() { /* Логика */ }

        [RelayCommand]
        private void OpenAllClusters() { /* Логика */ }
    }

    public partial class DashboardTaskViewModel : ObservableObject
    {
        [ObservableProperty] private string _uuid;
        [ObservableProperty] private string _name;
        [ObservableProperty] private string _status;
        [ObservableProperty] private int _progress;
    }

    public partial class DashboardLogViewModel : ObservableObject
    {
        [ObservableProperty] private string _timestamp;
        [ObservableProperty] private string _level;
        [ObservableProperty] private string _message;
    }

    public partial class DashboardProjectViewModel : ObservableObject
    {
        [ObservableProperty] private string _title;
        [ObservableProperty] private string _timeAgo;
    }

    public partial class DashboardClusterLoadViewModel : ObservableObject
    {
        [ObservableProperty] private string _nodeName;
        [ObservableProperty] private string _ip;
        [ObservableProperty] private int _loadPercentage;
    }
}