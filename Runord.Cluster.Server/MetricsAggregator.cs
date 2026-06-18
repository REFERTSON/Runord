using System.Collections.Concurrent;
using Runord.Cluster.Server.Grpc;

namespace Runord.Cluster.Server
{
    public class MetricsAggregator
    {
        // Храним последние показатели каждого воркера
        private readonly ConcurrentDictionary<string, HealthSnapshot> _workerMetrics = new();

        public void UpdateWorkerMetrics(HealthSnapshot snapshot)
        {
            _workerMetrics[snapshot.WorkerId] = snapshot;
        }

        public (double AverageCpu, double AverageMemory) GetClusterAverages()
        {
            if (_workerMetrics.IsEmpty) return (0, 0);

            var snapshots = _workerMetrics.Values.ToList();
            double avgCpu = snapshots.Average(x => x.CpuUsagePercentage);
            double avgMem = snapshots.Average(x => x.MemoryUsagePercentage);

            return (avgCpu, avgMem);
        }
    }
}
