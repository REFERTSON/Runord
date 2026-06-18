using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.Entities
{
    // Сущность, представляющая уведомление в системе.
    public class NotificationEntity : BaseEntity
    {
        // Идентификатор пользователя, которому предназначено это уведомление.
        public Guid? UserId { get; set; }
        public virtual UserEntity? User { get; set; } = null!;

        // Тип уведомления (информация, предупреждение, ошибка и т.д.).
        public NotificationType Type { get; set; } = NotificationType.Info;

        // Заголовок уведомления.
        public string Title { get; set; } = string.Empty;

        // Сообщение уведомления.
        public string Message { get; set; } = string.Empty;

        // Флаг, указывающий, было ли уведомление прочитано пользователем.
        public bool IsRead { get; set; }
    }
}