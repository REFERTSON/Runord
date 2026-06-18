using Runord.Shared.Enums;

namespace Runord.Shared.Entities
{
    // Класс для сообщений в очереди задач
    public class TaskQueueMessage
    {
        // Идентификатор задачи
        public Guid TaskId { get; set; }

        // Идентификатор владельца задачи
        public Guid OwnerId { get; set; }

        // Название задачи
        public string Name { get; set; } = string.Empty;

        // Время создания задачи
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        // Тип задачи (например, "ImageProcessing", "DataAnalysis" и т.д.)
        public string TaskType { get; set; } = string.Empty;

        // Приоритет задачи
        public TaskPriority Priority { get; set; } = TaskPriority.Low;

        // Параметры задачи в виде JSON-строки (может содержать любые необходимые параметры для выполнения задачи)
        public string ParametersJson { get; set; } = "{}";

        // Пути к входным файлам, если задача требует их (например, для обработки изображений или анализа данных)
        public List<string> InputFilePaths { get; set; } = new();
    }

    // Класс для сообщений о результатах выполнения задач
    public class TaskResultMessage
    {
        // Идентификатор задачи, для которой предназначено это сообщение
        public Guid TaskId { get; set; }
        // Успешность выполнения задачи
        public bool IsSuccess { get; set; } = true;
        // Сообщение об ошибке, если задача завершилась с ошибкой (null, если задача выполнена успешно)
        public string? ErrorMessage { get; set; } = null;
        // Результат выполнения задачи в виде JSON-строки.
        public List<string> OutputFilePaths { get; set; } = new();
        // Время выполнения задачи в секундах.
        public DateTimeOffset ExecutionTimeSeconds { get; set; }
        // Время завершения задачи
        public DateTimeOffset CompletedAt { get; set; }
    }
}
