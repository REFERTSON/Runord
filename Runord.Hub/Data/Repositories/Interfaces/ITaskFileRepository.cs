using Runord.Shared.Entities;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Data.Repositories.Interfaces
{
    public interface ITaskFileRepository : IBaseRepository<TaskFile>
    {
        Task<IEnumerable<TaskFile>> GetFilesByTaskIdAsync(Guid taskId, bool? isResult = null, CancellationToken cancellationToken = default);
        Task<TaskFile?> GetByMd5HashAsync(string md5Hash, CancellationToken cancellationToken = default);
    }
}