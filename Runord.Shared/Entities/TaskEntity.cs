using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.Entities
{
    // Сущность, представляющая задачу в системе.
    public class TaskEntity : BaseEntity
    {
        // Идентификатор проекта, к которому относится задача.
        public Guid ProjectId { get; set; }
        public virtual ProjectEntity? Project { get; set; }

        // Идентификатор пользователя, которому принадлежит задача.
        public Guid OwnerId { get; set; }
        public virtual UserEntity? Owner { get; set; }

        // Название задачи.
        public string Name { get; set; } = string.Empty;
        // Описание задачи.
        public string TaskType { get; set; } = string.Empty;
        // Параметры задачи в виде JSON-строки.
        public string ParametersJson { get; set; } = "{}";
        // Приоритет задачи.
        public TaskPriority Priority { get; set; }
        // Статус задачи.
        public Enums.TaskStatus Status { get; set; }
        // Процент выполнения задачи.
        public int ProgressPercent { get; set; }

        // Навигационные свойство для связанных файлов задачи.
        public virtual ICollection<TaskFileEntity> Files { get; set; } = new List<TaskFileEntity>();
    }
}