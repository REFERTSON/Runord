using Grpc.Core;
using Runord.Cluster.Server.Grpc;

namespace Runord.Cluster.Server.Services
{
    public class HealthReportingService : HealthReporter.HealthReporterBase
    {
        private readonly MetricsAggregator _aggregator;

        public HealthReportingService(MetricsAggregator aggregator)
        {
            _aggregator = aggregator;
        }

        public override Task<ReportAck> ReportHealth(HealthSnapshot request, ServerCallContext context)
        {
            _aggregator.UpdateWorkerMetrics(request);
            return Task.FromResult(new ReportAck { Success = true });
        }
    }
}
