using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Hub.Hubs;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Cluster;
using Runord.Shared.Entities;
using Runord.Shared.Enums;
using Runord.Shared.Filters;
using Runord.Shared.Interfaces;
using Runord.Shared.Interfaces.Services;

namespace Runord.Hub.Services
{
    public class ClusterService : IClusterService
    {
        private readonly IClusterRepository _clusterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ClusterHub> _clusterHubContext;

        public ClusterService(
            IClusterRepository clusterRepository,
            IUnitOfWork unitOfWork,
            IHubContext<ClusterHub> clusterHubContext)
        {
            _clusterRepository = clusterRepository;
            _unitOfWork = unitOfWork;
            _clusterHubContext = clusterHubContext;
        }

        private async Task NotifyClustersAsync(ClusterAction action, ClusterEntity cluster, CancellationToken ct = default)
        {
            var clusterDto = cluster.Adapt<ClusterDto>();
            var notification = new ClusterChangedNotification(action, clusterDto);
            await _clusterHubContext.Clients.Group("all_clusters")
                .SendAsync("ClusterChanged", notification, ct);
        }

        public async Task<Response<IEnumerable<ClusterDto>>> GetClustersAsync(ClusterFilter? filter = null, CancellationToken ct = default)
        {
            var query = _clusterRepository.GetQueryable();
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                    query = query.Where(c => c.Name.Contains(filter.SearchText) || c.IpAddress.Contains(filter.SearchText));
                if (filter.FromDate.HasValue)
                    query = query.Where(c => c.CreatedAt >= filter.FromDate.Value);
                if (filter.ToDate.HasValue)
                    query = query.Where(c => c.CreatedAt <= filter.ToDate.Value);
                if (filter.Status.HasValue)
                    query = query.Where(c => c.Status == filter.Status.Value);
            }
            var entities = await query.ToListAsync(ct);
            return Response<IEnumerable<ClusterDto>>.Success(entities.Adapt<IEnumerable<ClusterDto>>());
        }

        public async Task<Response<ClusterDto>> GetClusterByIdAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _clusterRepository.GetByIdAsync(id, ct);
            if (entity == null)
                return Response<ClusterDto>.Failure("Cluster not found");
            return Response<ClusterDto>.Success(entity.Adapt<ClusterDto>());
        }

        public async Task<Response<ClusterDto>> CreateClusterAsync(CreateClusterRequest request, CancellationToken ct = default)
        {
            var entity = new ClusterEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                IpAddress = request.IpAddress,
                Status = ClusterStatus.Offline,
                CreatedAt = DateTimeOffset.UtcNow
            };
            await _clusterRepository.AddAsync(entity, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await NotifyClustersAsync(ClusterAction.Create, entity, ct);

            return Response<ClusterDto>.Success(entity.Adapt<ClusterDto>());
        }

        public async Task<Response<ClusterDto>> UpdateClusterAsync(UpdateClusterRequest request, CancellationToken ct = default)
        {
            var entity = await _clusterRepository.GetByIdAsync(request.Id, ct);
            if (entity == null)
                return Response<ClusterDto>.Failure("Cluster not found");
            entity.Name = request.Name;
            entity.IpAddress = request.IpAddress;
            entity.LastModified = DateTimeOffset.UtcNow;
            _clusterRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            await NotifyClustersAsync(ClusterAction.Update, entity, ct);

            return Response<ClusterDto>.Success(entity.Adapt<ClusterDto>());
        }

        public async Task<Response<bool>> DeleteClusterAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _clusterRepository.GetByIdAsync(id, ct);
            if (entity == null)
                return Response<bool>.Failure("Cluster not found");

            var clusterDto = entity.Adapt<ClusterDto>();
            _clusterRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            await _clusterHubContext.Clients.Group("all_clusters")
                .SendAsync("ClusterChanged", new ClusterChangedNotification(ClusterAction.Delete, clusterDto), ct);

            return Response<bool>.Success(true);
        }

        // Внутренние методы
        public async Task UpdateClusterSpecsInternalAsync(Guid clusterId, double cpuTotalPercent, double ramTotalGb, CancellationToken ct)
        {

        }

        public async Task UpdateClusterMetricsInternalAsync(Guid clusterId, ClusterStatus status, double cpuUsagePercent, double ramUsageGb, CancellationToken ct)
        {

        }
    }
}