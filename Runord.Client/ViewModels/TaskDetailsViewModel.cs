using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Runord.Client.ViewModels.Base;
using System;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Runord.Client.ViewModels
{
    public partial class TaskDetailsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _taskId = string.Empty;

        [ObservableProperty]
        private string _taskName = string.Empty;

        [ObservableProperty]
        private string _status = string.Empty;

        [ObservableProperty]
        private int _progress;

        [ObservableProperty]
        private string _owner = string.Empty;

        [ObservableProperty]
        private string _priority = string.Empty;

        [ObservableProperty]
        private string _creationTime = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [RelayCommand]
        private void LoadTask(Guid id)
        {
            // Заглушка: загрузить данные задачи
            TaskId = id.ToString();
            TaskName = "Пример задачи";
            Status = "Выполняется";
            Progress = 50;
            Owner = "Система";
            Priority = "Высокий";
            CreationTime = DateTime.Now.ToString("g");
            Description = "Детальное описание задачи...";
        }

        [RelayCommand]
        private void CancelTask()
        {
            System.Windows.MessageBox.Show("Задача отменена", "Информация", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}