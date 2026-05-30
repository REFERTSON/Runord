using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.Entities
{
    public class TaskEntity : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public virtual ProjectEntity? Project { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TaskType { get; set; } = string.Empty;
        public Enums.TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public int ProgressPercent { get; set; }
        public Guid OwnerId { get; set; }
        public virtual UserEntity? Owner { get; set; }

        public virtual ICollection<TaskFile> Files { get; set; } = new List<TaskFile>();
    }
}