using Runord.Shared.Base;
using Runord.Shared.Entities;
using Runord.Shared.Filters;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Data.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<UserEntity>
    {
        Task<UserEntity?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<IEnumerable<UserEntity>> GetUsersAsync(UserFilter? filter, CancellationToken ct = default);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null, CancellationToken ct = default);
    }
}