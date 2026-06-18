using Runord.Shared.Entities;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Data.Repositories.Interfaces
{
    public interface ISettingRepository : IBaseRepository<SettingEntity>
    {
        Task<SettingEntity?> GetByKeyAsync(string key, CancellationToken ct = default);
        Task<IEnumerable<SettingEntity>> GetAllAsync(CancellationToken ct = default);
    }
}