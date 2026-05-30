using Runord.Shared.Enums;

namespace Runord.Shared.DTOs.Notification
{
    public class NotificationFilter
    {
        public bool? IsRead { get; set; }
        public NotificationType? Type { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string? SearchText { get; set; }
    }
}