using Microsoft.EntityFrameworkCore;
using Runord.Shared.Entities;
using Runord.Hub.Data.Repositories.Interfaces;

namespace Runord.Hub.Data.Repositories
{
    public class TaskFileRepository : BaseRepository<TaskFile>, ITaskFileRepository
    {
        public TaskFileRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskFile>> GetFilesByTaskIdAsync(Guid taskId, bool? isResult = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(f => f.TaskId == taskId);

            if (isResult.HasValue)
            {
                query = query.Where(f => f.IsResult == isResult.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TaskFile?> GetByMd5HashAsync(string md5Hash, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(md5Hash)) return null;
            return await _dbSet.FirstOrDefaultAsync(f => f.Md5Hash == md5Hash, cancellationToken);
        }
    }
}