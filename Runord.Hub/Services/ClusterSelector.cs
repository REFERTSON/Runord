using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Hub.Services.Interfaces;
using Runord.Shared.Entities;
using Runord.Shared.Enums;

namespace Runord.Hub.Services
{
    public class ClusterSelector : IClusterSelector
    {
        private readonly IClusterRepository _clusterRepository;

        public ClusterSelector(IClusterRepository clusterRepository)
        {
            _clusterRepository = clusterRepository;
        }

        public async Task<ClusterEntity?> SelectClusterAsync(string taskType, double requiredCpu, double requiredRam, CancellationToken cancellationToken = default)
        {
            var clusters = await _clusterRepository.GetClustersByStatusAsync(ClusterStatus.Online, cancellationToken);
            if (!clusters.Any())
                return null;

            var candidates = clusters
                .Where(c => (c.CpuTotal - (c.CpuUsagePercent / 100.0 * c.CpuTotal)) >= requiredCpu)
                .Where(c => (c.RamTotalGb - c.RamUsageGb) >= requiredRam)
                .ToList();

            return candidates.OrderBy(c => c.CpuUsagePercent).FirstOrDefault();
        }
    }
}