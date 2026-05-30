using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.Entities
{
    public class NotificationEntity : BaseEntity
    {
        public Guid? UserId { get; set; }
        public virtual UserEntity? User { get; set; } = null!;

        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
    }
}