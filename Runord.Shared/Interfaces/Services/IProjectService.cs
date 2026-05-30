using Runord.Shared.Base;
using Runord.Shared.DTOs.Project;

namespace Runord.Shared.Interfaces
{
    public interface IProjectService
    {
        Task<Result<PagedResponse<ProjectDto>>> GetProjectsAsync(Guid userId, bool isAdmin, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Result<ProjectDto>> GetProjectByIdAsync(Guid id, Guid userId, bool isAdmin, CancellationToken cancellationToken = default);
        Task<Result<ProjectDto>> CreateProjectAsync(CreateProjectRequest request, Guid userId, CancellationToken cancellationToken = default);
        Task<Result<ProjectDto>> UpdateProjectAsync(Guid id, UpdateProjectRequest request, Guid userId, bool isAdmin, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteProjectAsync(Guid id, Guid userId, bool isAdmin, CancellationToken cancellationToken = default);
    }
}