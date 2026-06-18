using Runord.Shared.Entities;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Data.Repositories.Interfaces
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshTokenEntity>
    {
        Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken ct = default);

        // Удаляет все токены пользователя при выходе
        Task DeleteAllForUserAsync(Guid userId, CancellationToken ct = default);

        // Метод для фонового смотрителя
        Task DeleteExpiredAndRevokedTokensAsync(CancellationToken ct = default);
    }
}