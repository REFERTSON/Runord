using CommunityToolkit.Mvvm.Input;
using Runord.Client.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Runord.Client.ViewModels
{
    public class TaskItem
    {
        public string Uuid { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; } // Например: "Ожидание", "Выполняется"
        public string Priority { get; set; } // Например: "Высокий"
        public string Files { get; set; } // Например: "2 Файла"
        public string CreationTime { get; set; } // "26/01/2026 - 15:45"
        public int Progress { get; set; } // Например: 0
    }

    public partial class TaskDispatherViewModel : ViewModelBase
    {
        // Коллекция задач, которая автоматически обновляет UI при добавлении/удалении
        public ObservableCollection<TaskItem> Tasks { get; set; }

        // Команда для удаления задачи

        public TaskDispatherViewModel()
        {
            // Заполнение коллекции 20 демонстрационными записями
            Tasks = new ObservableCollection<TaskItem>
            {
                new TaskItem { Uuid = Guid.NewGuid().ToString(), Owner = "Дмитриев Егор", Name = "Вычисление Пи", Type = "Вычисление ПИ", Status = "Ожидание", Priority = "Высокий", Files = "2 Файла", CreationTime = "26/01/2026 - 15:45", Progress = 0 },
                new TaskItem { Uuid = "4bf5rhnthj-02", Owner = "Дмитриев Егор", Name = "Вычисление Пи", Type = "Вычисление ПИ", Status = "Ожидание", Priority = "Высокий", Files = "2 Файла", CreationTime = "26/01/2026 - 15:45", Progress = 0 },
                new TaskItem { Uuid = "4bf5rhnthj-03", Owner = "Иванов Иван", Name = "Оптимизация БД", Type = "Тех. обслуживание", Status = "Выполняется", Priority = "Средний", Files = "1 Файл", CreationTime = "26/01/2026 - 16:00", Progress = 45 },
                new TaskItem { Uuid = "4bf5rhnthj-04", Owner = "Петров Алексей", Name = "Парсинг логов", Type = "Аналитика", Status = "Ошибка", Priority = "Высокий", Files = "5 Файлов", CreationTime = "26/01/2026 - 16:10", Progress = 12 },
                new TaskItem { Uuid = "4bf5rhnthj-05", Owner = "Сидоров Олег", Name = "Генерация отчетов", Type = "Документооборот", Status = "Отменено", Priority = "Низкий", Files = "0 Файлов", CreationTime = "26/01/2026 - 16:20", Progress = 0 },
                new TaskItem { Uuid = "4bf5rhnthj-06", Owner = "Дмитриев Егор", Name = "Вычисление Пи", Type = "Вычисление ПИ", Status = "Ожидание", Priority = "Высокий", Files = "2 Файла", CreationTime = "26/01/2026 - 16:30", Progress = 0 },
                new TaskItem { Uuid = "4bf5rhnthj-07", Owner = "Кузнецов Андрей", Name = "Сжатие видео", Type = "Медиа", Status = "Выполняется", Priority = "Низкий", Files = "1 Файл", CreationTime = "26/01/2026 - 16:40", Progress = 88 },
                new TaskItem { Uuid = "4bf5rhnthj-08", Owner = "Смирнов Дмитрий", Name = "Бэкап системы", Type = "Архивация", Status = "Выполнено", Priority = "Высокий", Files = "3 Файла", CreationTime = "26/01/2026 - 17:00", Progress = 100 },
                new TaskItem { Uuid = "4bf5rhnthj-09", Owner = "Попова Анна", Name = "Проверка прокси", Type = "Сеть", Status = "Ошибка", Priority = "Средний", Files = "1 Файл", CreationTime = "26/01/2026 - 17:15", Progress = 5 },
                new TaskItem { Uuid = "4bf5rhnthj-10", Owner = "Дмитриев Егор", Name = "Вычисление Пи", Type = "Вычисление ПИ", Status = "Ожидание", Priority = "Высокий", Files = "2 Файла", CreationTime = "26/01/2026 - 17:20", Progress = 0 },
                new TaskItem { Uuid = "4bf5rhnthj-11", Owner = "Васильев Игорь", Name = "Обучение нейросети", Type = "AI/ML", Status = "Выполняется", Priority = "Высокий", Files = "12 Файлов", CreationTime = "26/01/2026 - 17:30", Progress = 21 },
                new TaskItem { Uuid = "4bf5rhnthj-12", Owner = "Соколов Максим", Name = "Экспорт таблиц", Type = "Документооборот", Status = "Ожидание", Priority = "Низкий", Files = "1 Файл", CreationTime = "26/01/2026 - 17:45", Progress = 0 },
                new TaskItem { Uuid = "4bf5rhnthj-13", Owner = "Новиков Сергей", Name = "Скан уязвимостей", Type = "Безопасность", Status = "Выполняется", Priority = "Высокий", Files = "0 Файлов", CreationTime = "26/01/2026 - 18:00", Progress = 64 },
                new TaskItem { Uuid = "4bf5rhnthj-14", Owner = "Дмитриев Егор", Name = "Вычисление Пи", Type = "Вычисление ПИ", Status = "Ожидание", Priority = "Высокий", Files = "2 Файла", CreationTime = "26/01/2026 - 18:10", Progress = 0 },
                new TaskItem { Uuid = "4bf5rhnthj-15", Owner = "Морозов Илья", Name = "Импорт клиентов", Type = "Миграция данных", Status = "Отменено", Priority = "Средний", Files = "2 Файла", CreationTime = "26/01/2026 - 18:15", Progress = 0 },
                new TaskItem { Uuid = "4bf5rhnthj-16", Owner = "Волков Артем", Name = "Очистка кэша", Type = "Тех. обслуживание", Status = "Выполнено", Priority = "Низкий", Files = "0 Файлов", CreationTime = "26/01/2026 - 18:20", Progress = 100 },
                new TaskItem { Uuid = "4bf5rhnthj-17", Owner = "Федоров Денис", Name = "Генерация ключей", Type = "Криптография", Status = "Ожидание", Priority = "Высокий", Files = "1 Файл", CreationTime = "26/01/2026 - 18:40", Progress = 0 },
                new TaskItem { Uuid = "4bf5rhnthj-18", Owner = "Дмитриев Егор", Name = "Вычисление Пи", Type = "Вычисление ПИ", Status = "Ожидание", Priority = "Высокий", Files = "2 Файла", CreationTime = "26/01/2026 - 18:50", Progress = 0 },
                new TaskItem { Uuid = "4bf5rhnthj-19", Owner = "Белов Юрий", Name = "Проверка связи", Type = "Сеть", Status = "Выполняется", Priority = "Низкий", Files = "1 Файл", CreationTime = "26/01/2026 - 19:00", Progress = 95 },
                new TaskItem { Uuid = "4bf5rhnthj-20", Owner = "Гаврилов Олег", Name = "Сборка релизов", Type = "CI/CD", Status = "Ошибка", Priority = "Высокий", Files = "4 Файла", CreationTime = "26/01/2026 - 19:10", Progress = 50 }
            };
        }


        // Логика удаления элемента
        [RelayCommand]
        private void DeleteTask(object parameter)
        {
            if (parameter is TaskItem task && Tasks.Contains(task))
            {
                Tasks.Remove(task);
            }
        }
    }
}
