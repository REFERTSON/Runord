using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Runord.Client.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace Runord.Client.ViewModels
{
    public partial class TaskParameter : ObservableObject
    {
        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _subtitle;

        [ObservableProperty]
        private string _parameterType; // "Number", "String" или "File"

        [ObservableProperty]
        private string _value;
    }

    public partial class TaskBuilderViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _projectName = "Runord Core V2";

        [ObservableProperty]
        private string _taskName = "Умножение матриц";

        [ObservableProperty]
        private string _selectedType;

        [ObservableProperty]
        private string _selectedPriority;

        [ObservableProperty]
        private string _taskDescription = "Вычисление произведения двух больших матриц с использованием GPU-ускорения.";

        public ObservableCollection<string> TaskTypes { get; set; }
        public ObservableCollection<string> Priorities { get; set; }
        public ObservableCollection<TaskParameter> Parameters { get; set; }

        public TaskBuilderViewModel()
        {
            TaskTypes = new ObservableCollection<string>
            {
                "Вычисление ПИ", "Тех. обслуживание", "Аналитика данных", "Криптография", "Машинное обучение", "Математика"
            };

            Priorities = new ObservableCollection<string>
            {
                "Низкий", "Средний", "Высокий"
            };

            SelectedType = TaskTypes[5];
            SelectedPriority = Priorities[2];

            // Инициализация двух параметров-файлов для матриц
            Parameters = new ObservableCollection<TaskParameter>
            {
                new TaskParameter { Title = "МАТРИЦА A", Subtitle = "Укажите путь к первому бинарному файлу", ParameterType = "File", Value = "C:\\Users\\FREDDBEAR_FAZBEARSON\\Downloads\\ProjectCard.png" },
                new TaskParameter { Title = "МАТРИЦА B", Subtitle = "Укажите путь ко второму бинарному файлу", ParameterType = "File", Value = "C:\\Users\\FREDDBEAR_FAZBEARSON\\Downloads\\ProjectCard.png" }
            };
        }

        [RelayCommand]
        private void LaunchTask()
        {
            if (string.IsNullOrWhiteSpace(TaskName))
            {
                MessageBox.Show("Пожалуйста, укажите название задачи.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBox.Show($"Задача '{TaskName}' для проекта '{ProjectName}' успешно запущена!", "Runord", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void Save() => MessageBox.Show("Черновик задачи сохранен.");

        [RelayCommand]
        private void LoadTemplate() => MessageBox.Show("Открытие окна выбора шаблонов...");

        [RelayCommand]
        private void ClearAll()
        {
            ProjectName = string.Empty;
            TaskName = string.Empty;
            TaskDescription = string.Empty;
            foreach (var p in Parameters) p.Value = string.Empty;
        }

        [RelayCommand]
        private void Cancel() => MessageBox.Show("Отмена создания задачи...");
    }
}