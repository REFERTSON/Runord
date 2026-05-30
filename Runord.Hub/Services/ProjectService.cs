using Mapster;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Project;
using Runord.Shared.Entities;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResponse<ProjectDto>>> GetProjectsAsync(Guid userId, bool isAdmin, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var all = await _projectRepository.GetProjectsByUserIdAsync(userId, cancellationToken);
            if (!isAdmin)
                all = all.Where(p => p.CreatedById == userId);
            var totalCount = all.Count();
            var paged = all.Skip((page - 1) * pageSize).Take(pageSize);
            var dtos = paged.Adapt<List<ProjectDto>>();
            var response = new PagedResponse<ProjectDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
            return Result<PagedResponse<ProjectDto>>.Success(response);
        }

        public async Task<Result<ProjectDto>> GetProjectByIdAsync(Guid id, Guid userId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetProjectWithDetailsAsync(id, cancellationToken);
            if (project == null)
                return Result<ProjectDto>.Failure("Проект не найден");
            if (!isAdmin && project.CreatedById != userId)
                return Result<ProjectDto>.Failure("Нет прав");
            var dto = project.Adapt<ProjectDto>();
            return Result<ProjectDto>.Success(dto);
        }

        public async Task<Result<ProjectDto>> CreateProjectAsync(CreateProjectRequest request, Guid userId, CancellationToken cancellationToken = default)
        {
            var entity = new ProjectEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedById = userId,
                CreatedAt = DateTimeOffset.UtcNow
            };
            await _projectRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<ProjectDto>.Success(entity.Adapt<ProjectDto>());
        }

        public async Task<Result<ProjectDto>> UpdateProjectAsync(Guid id, UpdateProjectRequest request, Guid userId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
            if (project == null)
                return Result<ProjectDto>.Failure("Проект не найден");
            if (!isAdmin && project.CreatedById != userId)
                return Result<ProjectDto>.Failure("Нет прав");

            project.Name = request.Name;
            project.Description = request.Description;
            project.LastModified = DateTimeOffset.UtcNow;
            _projectRepository.Update(project);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<ProjectDto>.Success(project.Adapt<ProjectDto>());
        }

        public async Task<Result<bool>> DeleteProjectAsync(Guid id, Guid userId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
            if (project == null)
                return Result<bool>.Failure("Проект не найден");
            if (!isAdmin && project.CreatedById != userId)
                return Result<bool>.Failure("Нет прав");
            _projectRepository.Delete(project);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}