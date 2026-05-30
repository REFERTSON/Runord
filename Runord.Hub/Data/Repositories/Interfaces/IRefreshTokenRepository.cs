using Runord.Shared.Entities;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Data.Repositories.Interfaces
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshTokenEntity>
    {
        Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken ct = default);
        Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default);
    }
}