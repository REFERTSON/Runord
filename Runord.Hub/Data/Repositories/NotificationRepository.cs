using Microsoft.EntityFrameworkCore;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Notification;
using Runord.Shared.Entities;
using Runord.Hub.Data.Repositories.Interfaces;

namespace Runord.Hub.Data.Repositories
{
    public class NotificationRepository : BaseRepository<NotificationEntity>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context) { }

        public async Task<PagedResponse<NotificationEntity>> GetPagedUserNotificationsAsync(
            Guid userId,
            NotificationFilter filter,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(n => n.UserId == userId);

            if (filter.IsRead.HasValue)
                query = query.Where(n => n.IsRead == filter.IsRead.Value);
            if (filter.Type.HasValue)
                query = query.Where(n => n.Type == filter.Type.Value);
            if (filter.FromDate.HasValue)
                query = query.Where(n => n.CreatedAt >= filter.FromDate.Value);
            if (filter.ToDate.HasValue)
                query = query.Where(n => n.CreatedAt <= filter.ToDate.Value);
            if (!string.IsNullOrWhiteSpace(filter.SearchText))
                query = query.Where(n => n.Title.Contains(filter.SearchText) || n.Message.Contains(filter.SearchText));

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<NotificationEntity>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.IsRead, true), cancellationToken);

        public async Task<int> DeleteAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet
                .Where(n => n.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);

        public async Task AddRangeAsync(IEnumerable<NotificationEntity> entities, CancellationToken cancellationToken = default)
            => await _dbSet.AddRangeAsync(entities, cancellationToken);
    }
}