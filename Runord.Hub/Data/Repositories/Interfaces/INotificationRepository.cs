using Runord.Shared.Base;
using Runord.Shared.Entities;
using Runord.Shared.Filters;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Data.Repositories.Interfaces
{
    public interface INotificationRepository : IBaseRepository<NotificationEntity>
    {
        // Пагинация + фильтрация (возвращает PagedResponse)
        Task<IEnumerable<NotificationEntity>> GetUserNotificationsAsync(
            Guid userId,
            NotificationFilter filter,
            CancellationToken cancellationToken = default);

        // Массовое обновление (отметить все как прочитанные)
        Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);

        // Массовое удаление всех уведомлений пользователя
        Task<int> DeleteAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}