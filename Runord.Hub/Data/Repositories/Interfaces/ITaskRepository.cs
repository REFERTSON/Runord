using Runord.Shared.Entities;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Data.Repositories.Interfaces
{
    public interface ITaskRepository : IBaseRepository<TaskEntity>
    {
        Task<TaskEntity?> GetTaskWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskEntity>> GetFilteredTasksAsync(
            Guid? userId, bool isAdmin, Guid? projectId, CancellationToken cancellationToken = default);
    }
}