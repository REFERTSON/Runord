using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runord.Client.Shared.Enums
{
    public enum TaskItemStatus
    {
        Pending,    // Ожидание
        Running,    // Выполнение
        Completed,  // Завершено
        Failed,     // Ошибка
        Cancelled   // Отменена
    }

    public static class TaskItemStatusDescriptions
    {
        public static readonly IReadOnlyDictionary<TaskItemStatus, string> Descriptions = new Dictionary<TaskItemStatus, string>
        {
            [TaskItemStatus.Pending] = "Ожидание",
            [TaskItemStatus.Running] = "Выполнение",
            [TaskItemStatus.Completed] = "Завершено",
            [TaskItemStatus.Failed] = "Ошибка",
            [TaskItemStatus.Cancelled] = "Отменена"
        };

        public static string GetDescription(TaskItemStatus status)
            => Descriptions.TryGetValue(status, out var desc) ? desc : status.ToString();
    }

}
