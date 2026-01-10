using Runord.Client.App.Base;
using Runord.Client.App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Runord.Client.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private FrameworkElement _currentView;
        public FrameworkElement CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        // View для страниц
        public FrameworkElement Dashboard { get; } = new DashboardView();
        public FrameworkElement TaskBuilder { get; } = new TaskBuilderView();
        public FrameworkElement TaskHistory { get; } = new TaskHistoryView();
        public FrameworkElement HubDashboard { get; } = new HubDashboardView();
        public FrameworkElement UserManagement { get; } = new UserManagementView();
        public FrameworkElement NodeManagementt { get; } = new NodeManagementView();
        public FrameworkElement Console { get; } = new ConsoleView();
        public FrameworkElement Settings { get; } = new SettingsView();

        // Команды для переключения
        public ICommand ShowDashboardCommand { get; }
        public ICommand ShowTaskBuilderCommand { get; }

        public ICommand ShowTaskHistoryCommand { get; }
        public ICommand ShowHubDashboardCommand { get; }
        public ICommand ShowUserManagementCommand { get; }
        public ICommand ShowNodeManagementCommand { get; }
        public ICommand ShowConsoleCommand { get; }
        public ICommand ShowSettingsCommand { get; }

        public MainViewModel()
        {
            CurrentView = new DashboardView(); // стартовая страница

            ShowDashboardCommand = new RelayCommand(_ => CurrentView = Dashboard);
            ShowTaskBuilderCommand = new RelayCommand(_ => CurrentView = TaskBuilder);
            ShowTaskHistoryCommand = new RelayCommand(_ => CurrentView = TaskHistory);
            ShowHubDashboardCommand = new RelayCommand(_ => CurrentView = HubDashboard);
            ShowUserManagementCommand = new RelayCommand(_ => CurrentView = UserManagement);
            ShowNodeManagementCommand = new RelayCommand(_ => CurrentView = NodeManagementt);
            ShowConsoleCommand = new RelayCommand(_ => CurrentView = Console);
            ShowSettingsCommand = new RelayCommand(_ => CurrentView = Settings);
        }
    }
}
