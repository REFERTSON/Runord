using Runord.Shared.Entities;
using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Runord.Client.ViewModels.Base;

namespace Runord.Client.ViewModels
{
    public partial class ProjectDetailsViewModel : ViewModelBase // Предполагаю использование CommunityToolkit.Mvvm
    {
        // Сущность проекта для биндинга шапки
        public ProjectEntity Project { get; set; }

        // Статистика для карточек
        public int ActiveTasksCount { get; set; } = 4;
        public int CompletedTasksCount { get; set; } = 8;
        public int ErrorTasksCount { get; set; } = 2;

        // Те самые полноценные задачи
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public ProjectDetailsViewModel()
        {
            // Инициализация мок-проекта
            Project = new ProjectEntity
            {
                Id = Guid.NewGuid(),
                Name = "Обработка системных логов",
                Description = "Парсинг, фильтрация и структурирование терабайтных журналов со всех нод кластера за последний месяц. Проект включает настройку пайплайнов для Elasticsearch.",
                TaskCount = 14
            };

            // Инициализация задач (с полным набором свойств)
            Tasks = new ObservableCollection<TaskItem>
            {
                new TaskItem { Uuid = "4bf5rhnthj-01", Owner = "Дмитриев Егор", Name = "Вычисление Пи", Type = "Вычисление ПИ", Status = "Ожидание", Priority = "Высокий", Files = "2 Файла", Progress = 0 },
                new TaskItem { Uuid = "4bf5rhnthj-02", Owner = "Иванов Иван", Name = "Оптимизация БД", Type = "Тех. обслуживание", Status = "Выполняется", Priority = "Средний", Files = "1 Файл", Progress = 45 },
                new TaskItem { Uuid = "4bf5rhnthj-03", Owner = "Петров Алексей", Name = "Парсинг логов", Type = "Аналитика", Status = "Ошибка", Priority = "Высокий", Files = "5 Файлов", Progress = 12 },
                new TaskItem { Uuid = "4bf5rhnthj-04", Owner = "Сидоров Олег", Name = "Генерация отчетов", Type = "Документооборот", Status = "Отменено", Priority = "Низкий", Files = "0 Файлов", Progress = 0 },
                new TaskItem { Uuid = "4bf5rhnthj-05", Owner = "Смирнов Дмитрий", Name = "Бэкап системы", Type = "Архивация", Status = "Выполнено", Priority = "Средний", Files = "3 Файла", Progress = 100 }
            };
        }
    }
}