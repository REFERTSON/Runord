using Runord.Shared.Entities;

namespace Runord.Hub.Services.Interfaces
{
    public interface IClusterSelector
    {
        Task<ClusterEntity?> SelectClusterAsync(string taskType, double requiredCpu, double requiredRam, CancellationToken cancellationToken = default);
    }
}