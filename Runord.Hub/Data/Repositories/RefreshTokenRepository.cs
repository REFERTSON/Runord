using Microsoft.EntityFrameworkCore;
using Runord.Shared.Entities;
using Runord.Hub.Data.Repositories.Interfaces;

namespace Runord.Hub.Data.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshTokenEntity>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context) { }

        public async Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(rt => rt.Token == token, ct);

        public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
            => await _dbSet
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ExecuteUpdateAsync(setters => setters.SetProperty(rt => rt.RevokedAt, DateTimeOffset.UtcNow), ct);
    }
}