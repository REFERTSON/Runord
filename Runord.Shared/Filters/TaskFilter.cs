using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.Filters
{
    public class TaskFilter : BaseFilter
    {
        public Enums.TaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public Guid? OwnerId { get; set; }
    }
}