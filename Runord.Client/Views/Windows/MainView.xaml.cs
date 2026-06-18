using Runord.Client.UIElements.Controls;
using Runord.Client.ViewModels;
using Runord.Client.Views.Pages;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Runord.Client.Views.Windows
{
    public partial class MainView : UserControl
    {
        private readonly Dictionary<Type, Func<UserControl>> _viewMapping;
        private readonly MainViewModel _viewModel;

        public MainView()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            _viewMapping = new Dictionary<Type, Func<UserControl>>
            {
                { typeof(DashboardViewModel), () => new DashboardView() },
                { typeof(ProjectsListViewModel), () => new ProjectsListView() },
                { typeof(TaskBuilderViewModel), () => new TaskBuilderView() },
                { typeof(TaskDispatherViewModel), () => new TaskDispatherView() },
                { typeof(ClusterListViewModel), () => new ClusterListView() },
                { typeof(UserManagementViewModel), () => new UserManagementView() },
                { typeof(NotificationCenterViewModel), () => new NotificationCenterView() },
                { typeof(SettingsViewModel), () => new SettingsView() },
                { typeof(UserProfileViewModel), () => new UserProfileView() },
                { typeof(ProjectDetailsViewModel), () => new ProjectDetailsView() },
                { typeof(TaskDetailsViewModel), () => new TaskDetailsView() }
            };

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            Sidebar.NavigateRequested += Sidebar_NavigateRequested;

            ResolveAndApplyView();
        }

        private void Sidebar_NavigateRequested(object? sender, NavigationType navType)
        {
            if (_viewModel.NavigateCommand.CanExecute(navType))
                _viewModel.NavigateCommand.Execute(navType);
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.CurrentViewModel))
                ResolveAndApplyView();
        }

        private void ResolveAndApplyView()
        {
            if (_viewModel.CurrentViewModel == null)
            {
                MainContent.Content = null;
                return;
            }

            Type viewModelType = _viewModel.CurrentViewModel.GetType();

            if (_viewMapping.TryGetValue(viewModelType, out var createView))
            {
                UserControl targetView = createView();
                targetView.DataContext = _viewModel.CurrentViewModel;

                targetView.Opacity = 0;
                var transform = new TranslateTransform(0, 15);
                targetView.RenderTransform = transform;

                MainContent.Content = targetView;

                var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(350))
                { EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut } };
                var slideUpAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(350))
                { EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut } };

                targetView.BeginAnimation(OpacityProperty, fadeInAnimation);
                transform.BeginAnimation(TranslateTransform.YProperty, slideUpAnimation);
            }
        }
    }
}