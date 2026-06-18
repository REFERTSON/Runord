using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.Filters
{
    public class NotificationFilter : BaseFilter
    {
        public NotificationType? Type { get; set; }
        public bool? IsRead { get; set; }
    }
}