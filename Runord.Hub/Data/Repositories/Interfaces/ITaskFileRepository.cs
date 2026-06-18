using Runord.Shared.Entities;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Data.Repositories.Interfaces
{
    public interface ITaskFileRepository : IBaseRepository<TaskFileEntity>
    {
        Task<IEnumerable<TaskFileEntity>> GetFilesByTaskIdAsync(Guid taskId, bool? isResult = null, CancellationToken cancellationToken = default);
        Task<TaskFileEntity?> GetByMd5HashAsync(string md5Hash, CancellationToken cancellationToken = default);
    }
}