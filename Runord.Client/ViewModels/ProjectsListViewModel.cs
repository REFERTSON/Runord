using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Runord.Client.Data.Interfaces;
using Runord.Client.ViewModels.Base;
using Runord.Shared.DTOs.Project;
using Runord.Shared.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Runord.Client.ViewModels
{
    public partial class ProjectsListViewModel : ViewModelBase
    {
        private readonly IProjectService _projectService;
        private readonly IUserSessionStorage _sessionStorage;

        [ObservableProperty] private ObservableCollection<ProjectItemViewModel> _projectsList = new();
        [ObservableProperty] private string _bannerHeader = "";
        [ObservableProperty] private string _bannerMessage = "";
        [ObservableProperty] private bool _isAddDialogVisible;
        [ObservableProperty] private string _newProjectName = "";
        [ObservableProperty] private string _newProjectDescription = "";

        private readonly Random _random = new();
        private readonly (string Header, string Message)[] _bannerVariants = new[]
        {
            ("Проекты не найдены  (•_•)", "У вас пока что нету проектов. Создайте проект, чтобы начать работу с Runord."),
            ("Здесь подозрительно пусто...  (〃ー〃)", "Похоже, вы еще не запланировали ни одной задачи. Нажмите кнопку создания проекта, чтобы исправить это."),
            ("Время великих открытий!  (˚Δ˚)b", "Список проектов абсолютно чист. Самое время нажать кнопку «Создать проект» и развернуть новые вычисления."),
            ("Чертежи еще не готовы?  (─__─)", "Хаб готов к обработке, но выполнять пока нечего. Создайте свой первый рабочий проект сверху справа."),
            ("Проекты ушли в оффлайн  (ノ_<)", "В базе данных не обнаружено ни одной активной сессии. Добавьте новый проект для старта параллельных вычислений."),
            ("Чистый лист для новых идей  ( o^ ^o )", "Здесь пока нет ни одной записи. Нажмите кнопку создания сверху, чтобы задать конфигурацию для кластеров."),
            ("Куда все пропало?  (  •ิ_•ิ)?", "Текущий список проектов пуст. Если вы стерли старые задачи, нажмите «Создать проект», чтобы начать заново."),
            ("Система ожидает ваши задачи  (︶▽︶)", "Ни одного активного проекта прямо сейчас. Запустите новый рабочий процесс через верхнюю управляющую панель."),
            ("Вдохновение покинуло чат...  (｡T ω T｡)", "Потому что список ваших проектов пуст. Добавьте задачу, чтобы задействовать мощности подключенных кластеров."),
            ("Пора создать что-то масштабное!  ٩(◕_◕)۶", "Все старые проекты завершены или удалены. Нажмите кнопку сверху и соберите новую конфигурацию вычислений.")
        };

        public ProjectsListViewModel(IProjectService projectService, IUserSessionStorage sessionStorage)
        {
            _projectService = projectService;
            _sessionStorage = sessionStorage;
            LoadProjects();
            PickRandomBanner();
        }

        private async void LoadProjects()
        {
            await ExecuteAsync(async () =>
            {
                var session = await _sessionStorage.GetCurrentSessionAsync();
                if (session == null) return;
                var result = await _projectService.GetProjectsAsync(session.UserId, null, CancellationToken.None);
                if (result.IsSuccess && result.Data != null)
                {
                    ProjectsList.Clear();
                    foreach (var dto in result.Data)
                        ProjectsList.Add(new ProjectItemViewModel(dto));
                }
            });
        }

        private void PickRandomBanner()
        {
            var idx = _random.Next(_bannerVariants.Length);
            BannerHeader = _bannerVariants[idx].Header;
            BannerMessage = _bannerVariants[idx].Message;
        }

        public bool IsListEmpty => ProjectsList.Count == 0;

        [RelayCommand]
        private void OpenAddDialog()
        {
            NewProjectName = "";
            NewProjectDescription = "";
            IsAddDialogVisible = true;
        }
        [RelayCommand] private void CloseAddDialog() => IsAddDialogVisible = false;
        [RelayCommand]
        private async Task ConfirmAddProject()
        {
            if (string.IsNullOrWhiteSpace(NewProjectName)) return;
            var session = await _sessionStorage.GetCurrentSessionAsync();
            if (session == null) return;
            var request = new CreateProjectRequest(NewProjectName.Trim(), NewProjectDescription.Trim());
            var result = await _projectService.CreateProjectAsync(request, session.UserId, CancellationToken.None);
            if (result.IsSuccess && result.Data != null)
            {
                ProjectsList.Add(new ProjectItemViewModel(result.Data));
                IsAddDialogVisible = false;
            }
        }
        [RelayCommand]
        private async Task DeleteProject(ProjectItemViewModel project)
        {
            if (project == null) return;
            var session = await _sessionStorage.GetCurrentSessionAsync();
            if (session == null) return;
            var result = await _projectService.DeleteProjectAsync(project.Id, session.UserId, false, CancellationToken.None);
            if (result.IsSuccess)
                ProjectsList.Remove(project);
        }
    }

    public partial class ProjectItemViewModel : ObservableObject
    {
        public Guid Id { get; set; }
        [ObservableProperty] private string _name = "";
        [ObservableProperty] private string _description = "";
        [ObservableProperty] private int _taskCount;

        public ProjectItemViewModel() { }
        public ProjectItemViewModel(ProjectDto dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Description = dto.Description;
            TaskCount = dto.TaskCount;
        }
    }
}