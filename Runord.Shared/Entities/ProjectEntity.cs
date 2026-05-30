using Runord.Shared.Base;

namespace Runord.Shared.Entities
{
    public class ProjectEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Guid CreatedById { get; set; }
        public virtual UserEntity? CreatedBy { get; set; }

        public virtual ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
    }
}