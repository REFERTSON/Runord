using Runord.Shared.Base;
using Runord.Shared.DTOs.Cluster;

namespace Runord.Shared.Interfaces.Services
{
    public interface IClusterService
    {
        Task<Result<IEnumerable<ClusterDto>>> GetAllClustersAsync(CancellationToken cancellationToken = default);
        Task<Result<ClusterDto>> GetClusterByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<ClusterDto>> CreateClusterAsync(CreateClusterRequest request, CancellationToken cancellationToken = default);
        Task<Result<ClusterDto>> UpdateClusterAsync(UpdateClusterRequest request, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteClusterAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<bool>> UpdateClusterMetricsAsync(ClusterMetricsDto metrics, CancellationToken cancellationToken = default);
    }
}