using Microsoft.EntityFrameworkCore;
using Runord.Shared.Entities;
using Runord.Hub.Data.Repositories.Interfaces;

namespace Runord.Hub.Data.Repositories
{
    public class TaskRepository : BaseRepository<TaskEntity>, ITaskRepository
    {
        public TaskRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<TaskEntity?> GetTaskWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Files)
                .Include(t => t.Project)
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<(IEnumerable<TaskEntity> Items, int TotalCount)> GetFilteredTasksAsync(
            Guid? userId, bool isAdmin, Guid? projectId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (!isAdmin && userId.HasValue)
                query = query.Where(t => t.OwnerId == userId.Value);

            if (projectId.HasValue)
                query = query.Where(t => t.ProjectId == projectId.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}