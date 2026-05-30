using Runord.Shared.Entities;
using Runord.Shared.Enums;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Data.Repositories.Interfaces
{
    public interface IClusterRepository : IBaseRepository<ClusterEntity>
    {
        Task<ClusterEntity?> GetByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default);
        Task<IEnumerable<ClusterEntity>> GetClustersByStatusAsync(ClusterStatus status, CancellationToken cancellationToken = default);
    }
}