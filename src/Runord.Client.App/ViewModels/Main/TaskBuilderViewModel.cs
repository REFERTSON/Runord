using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Runord.Client.Shared.Enums;
using Runord.Client.App.Base;

namespace Runord.Client.App.ViewModels
{
    public class TaskBuilderViewModel : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private TaskItemType _selectedTaskTypeKey;
        public TaskItemType SelectedTaskTypeKey
        {
            get => _selectedTaskTypeKey;
            set => SetProperty(ref _selectedTaskTypeKey, value);
        }

        private string _selectedPriority;
        public string SelectedPriority
        {
            get => _selectedPriority;
            set => SetProperty(ref _selectedPriority, value);
        }

        private string _selectedExecutor;
        public string SelectedExecutor
        {
            get => _selectedExecutor;
            set => SetProperty(ref _selectedExecutor, value);
        }

        public ObservableCollection<KeyValuePair<TaskItemType, string>> TaskTypes { get; }

        public ObservableCollection<string> Priorities { get; }

        public ObservableCollection<string> Executors { get; }

        public ICommand AddParameterCommand { get; }
        public ICommand RemoveParameterCommand { get; }
        public ICommand SaveTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        public TaskBuilderViewModel()
        {
            // Инициализация коллекций
            TaskTypes = new ObservableCollection<KeyValuePair<TaskItemType, string>>(
                TaskItemTypeDescriptions.Descriptions
            );

            Priorities = new ObservableCollection<string>
            {
                "Низкий",
                "Средний",
                "Высокий",
                "Критический"
            };

            Executors = new ObservableCollection<string>
            {
                "Исполнитель 1",
                "Исполнитель 2",
                "Исполнитель 3",
                "Автоматическое назначение"
            };

            // Инициализация команд
            // AddParameterCommand = new RelayCommand(AddParameter);
            // RemoveParameterCommand = new RelayCommand(RemoveParameter);
            // SaveTaskCommand = new RelayCommand(SaveTask);
            // DeleteTaskCommand = new RelayCommand(DeleteTask);

            // Установка значений по умолчанию
            SelectedPriority = Priorities.First();
            SelectedExecutor = Executors.First();
            SelectedTaskTypeKey = TaskTypes.First().Key; // Установка первого типа задачи
        }

        private void AddParameter()
        {
        }

        private void RemoveParameter(object parameter)
        {
        }

        private void SaveTask()
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(Title))
            {
                // Показать сообщение об ошибке
                return;
            }

            // Вызов сервиса для сохранения
            // _taskService.SaveTask(task);

            // Сообщение об успехе
        }

        private void DeleteTask()
        {
            // Логика удаления задачи
            // _taskService.DeleteTask(taskId);

            // Закрытие окна или навигация назад
        }
    }
}