using Microsoft.EntityFrameworkCore;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Shared.Base;
using Runord.Shared.Entities;
using Runord.Shared.Filters;

namespace Runord.Hub.Data.Repositories
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<IEnumerable<UserEntity>> GetUsersAsync(UserFilter? filter, CancellationToken ct = default)
        {
            var query = _dbSet.AsNoTracking();

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                    query = query.Where(u => u.FullName.Contains(filter.SearchText) || u.Email.Contains(filter.SearchText));

                if (filter.IsBlocked.HasValue)
                    query = query.Where(u => u.IsBlocked == filter.IsBlocked.Value);

                if (!string.IsNullOrWhiteSpace(filter.Role))
                    query = query.Where(u => u.Role.ToString() == filter.Role);

                if (filter.FromDate.HasValue)
                    query = query.Where(u => u.CreatedAt >= filter.FromDate.Value);

                if (filter.ToDate.HasValue)
                    query = query.Where(u => u.CreatedAt <= filter.ToDate.Value);
            }

            return await query.OrderBy(u => u.Email).ToListAsync(ct);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null, CancellationToken ct = default)
        {
            var query = _dbSet.Where(u => u.Email == email);

            if (excludeUserId.HasValue)
                query = query.Where(u => u.Id != excludeUserId.Value);

            return !await query.AnyAsync(ct);
        }
    }
}