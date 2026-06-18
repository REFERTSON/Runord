using Microsoft.EntityFrameworkCore;
using Runord.Shared.Entities;
using Runord.Shared.Enums;
using Runord.Hub.Data.Repositories.Interfaces;

namespace Runord.Hub.Data.Repositories
{
    public class ClusterRepository : BaseRepository<ClusterEntity>, IClusterRepository
    {
        public ClusterRepository(AppDbContext context) : base(context) { }

        public async Task<ClusterEntity?> GetByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ipAddress)) return null;
            return await _dbSet.FirstOrDefaultAsync(c => c.IpAddress == ipAddress, cancellationToken);
        }

        public async Task<IEnumerable<ClusterEntity>> GetClustersByStatusAsync(ClusterStatus status, CancellationToken cancellationToken = default)
            => await _dbSet.Where(c => c.Status == status).ToListAsync(cancellationToken);
    }
}