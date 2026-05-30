using Runord.Shared.Entities;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Data.Repositories.Interfaces
{
    public interface IProjectRepository : IBaseRepository<ProjectEntity>
    {
        Task<ProjectEntity?> GetProjectWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProjectEntity>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}