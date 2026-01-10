using Runord.Client.App.Base;
using Runord.Client.App.Storage.Entities;
using Runord.Client.Shared.ValueObjects;
using Runord.Client.Shared.ValueObjects.Base;
using Runord.Client.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Data;

namespace Runord.Client.App.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        public ObservableCollection<TaskItem> Tasks { get; set; }

        private DataTable _table;
        public DataTable Table
        {
            get => _table;
            set
            {
                _table = value;
                OnPropertyChanged(nameof(Table));
            }
        }

        public DashboardViewModel()
        {
            // Пример инициализации
            Tasks = new ObservableCollection<TaskItem>
            {
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid()),
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid()),
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid()),
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid()),
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid()),
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid()),
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid()),
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid()),
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid()),
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid()),
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid()),
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid()),
                new TaskItem("Умножение матриц", TaskItemType.MatrixMultiplication, new TaskItemParameterBase(), Guid.NewGuid())
            };

            Table = new DataTable();
            Table.Columns.Add("UUID");
            Table.Columns.Add("Название");
            Table.Columns.Add("Тип задачи");
            Table.Columns.Add("Статус");
            Table.Columns.Add("Приоритет");
            Table.Columns.Add("Приложение");
            Table.Columns.Add("Дата создания");
            Table.Columns.Add(" ");
        }


    }
}
