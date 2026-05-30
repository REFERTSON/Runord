using Microsoft.EntityFrameworkCore;
using Runord.Shared.Base;
using Runord.Shared.Entities;
using Runord.Hub.Data.Repositories.Interfaces;

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

        public async Task<PagedResponse<UserEntity>> GetPagedUsersAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var query = _dbSet.AsNoTracking();
            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
            return new PagedResponse<UserEntity>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
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