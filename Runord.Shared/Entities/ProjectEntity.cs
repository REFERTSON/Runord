using Runord.Shared.Base;

namespace Runord.Shared.Entities
{
    // Сущность проекта, которая содержит информацию о проекте, его задачах и создателе
    public class ProjectEntity : BaseEntity
    {
        // Название проекта
        public string Name { get; set; } = string.Empty;
        // Описание проекта
        public string Description { get; set; } = string.Empty;
        // Количество задач в проекте
        public int TaskCount { get; set; } = 0;

        // Идентификатор пользователя, который создал проект
        public Guid CreatedById { get; set; }
        public virtual UserEntity? CreatedBy { get; set; }

        // Навигационное свойство для задач, связанных с этим проектом
        public virtual ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
    }
}