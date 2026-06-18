using Runord.Shared.Base;
using Runord.Shared.DTOs.Cluster;
using Runord.Shared.Enums;
using Runord.Shared.Filters;

namespace Runord.Shared.Interfaces.Services
{
    public interface IClusterService
    {
        // Для WPF-клиента (REST API)
        Task<Response<IEnumerable<ClusterDto>>> GetClustersAsync(ClusterFilter? filter = null, CancellationToken ct = default);
        Task<Response<ClusterDto>> GetClusterByIdAsync(Guid id, CancellationToken ct = default);
        Task<Response<ClusterDto>> CreateClusterAsync(CreateClusterRequest request, CancellationToken ct = default);
        Task<Response<ClusterDto>> UpdateClusterAsync(UpdateClusterRequest request, CancellationToken ct = default);
        Task<Response<bool>> DeleteClusterAsync(Guid id, CancellationToken ct = default);
    }
}