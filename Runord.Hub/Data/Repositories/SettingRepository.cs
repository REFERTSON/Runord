using Microsoft.EntityFrameworkCore;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Shared.Entities;

namespace Runord.Hub.Data.Repositories
{
    public class SettingRepository : BaseRepository<SettingEntity>, ISettingRepository
    {
        public SettingRepository(AppDbContext context) : base(context) { }

        public async Task<SettingEntity?> GetByKeyAsync(string key, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(s => s.Key == key, ct);

        public async Task<IEnumerable<SettingEntity>> GetAllAsync(CancellationToken ct = default)
            => await _dbSet.ToListAsync(ct);
    }
}