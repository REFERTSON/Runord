using Mapster;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Cluster;
using Runord.Shared.Entities;
using Runord.Shared.Enums;
using Runord.Shared.Interfaces;
using Runord.Shared.Interfaces.Services;

namespace Runord.Hub.Services
{
    public class ClusterService : IClusterService
    {
        private readonly IClusterRepository _clusterRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ClusterService(IClusterRepository clusterRepository, IUnitOfWork unitOfWork)
        {
            _clusterRepository = clusterRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<ClusterDto>>> GetAllClustersAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _clusterRepository.GetAllAsync(cancellationToken);
            var dtos = entities.Adapt<IEnumerable<ClusterDto>>();
            return Result<IEnumerable<ClusterDto>>.Success(dtos);
        }

        public async Task<Result<ClusterDto>> GetClusterByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _clusterRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return Result<ClusterDto>.Failure("Узел не найден");
            var dto = entity.Adapt<ClusterDto>();
            return Result<ClusterDto>.Success(dto);
        }

        public async Task<Result<ClusterDto>> CreateClusterAsync(CreateClusterRequest request, CancellationToken cancellationToken = default)
        {
            var entity = request.Adapt<ClusterEntity>();
            entity.Id = Guid.NewGuid();
            entity.Status = ClusterStatus.Offline;
            entity.CreatedAt = DateTimeOffset.UtcNow;
            await _clusterRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<ClusterDto>.Success(entity.Adapt<ClusterDto>());
        }

        public async Task<Result<ClusterDto>> UpdateClusterAsync(UpdateClusterRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _clusterRepository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
                return Result<ClusterDto>.Failure("Узел не найден");

            request.Adapt(entity);
            entity.LastModified = DateTimeOffset.UtcNow;
            _clusterRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<ClusterDto>.Success(entity.Adapt<ClusterDto>());
        }

        public async Task<Result<bool>> DeleteClusterAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _clusterRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return Result<bool>.Failure("Узел не найден");
            _clusterRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdateClusterMetricsAsync(ClusterMetricsDto metrics, CancellationToken cancellationToken = default)
        {
            var node = await _clusterRepository.GetByIdAsync(metrics.NodeId, cancellationToken);
            if (node == null)
                return Result<bool>.Failure("Узел не найден");

            node.CpuUsagePercent = metrics.CpuUsagePercent;
            node.RamUsageGb = metrics.RamUsageGb;
            node.StorageUsageGb = metrics.StorageUsageGb;
            node.NetworkInMbps = metrics.NetworkInMbps;
            node.NetworkOutMbps = metrics.NetworkOutMbps;
            node.Status = metrics.Status;
            node.LastModified = DateTimeOffset.UtcNow;

            _clusterRepository.Update(node);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}