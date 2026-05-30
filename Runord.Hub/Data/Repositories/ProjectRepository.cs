using Microsoft.EntityFrameworkCore;
using Runord.Shared.Entities;
using Runord.Hub.Data.Repositories.Interfaces;

namespace Runord.Hub.Data.Repositories
{
    public class ProjectRepository : BaseRepository<ProjectEntity>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ProjectEntity?> GetProjectWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.CreatedBy)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ProjectEntity>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.CreatedById == userId)
                .Include(p => p.Tasks)
                .ToListAsync(cancellationToken);
        }
    }
}