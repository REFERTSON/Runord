using Mapster;
using Microsoft.AspNetCore.SignalR;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Hub.Hubs;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Project;
using Runord.Shared.Entities;
using Runord.Shared.Filters;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ProjectHub> _projectHubContext;

        public ProjectService(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IHubContext<ProjectHub> projectHubContext)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _projectHubContext = projectHubContext;
        }

        private async Task NotifyProjectChangedAsync(string action, ProjectEntity project, CancellationToken ct)
        {
            var dto = project.Adapt<ProjectDto>();
            // Уведомляем всех, кто подписан на этот проект (владельца, участников)
            await _projectHubContext.Clients.Group($"project_{project.Id}")
                .SendAsync("ProjectChanged", action, dto, ct);
        }

        public async Task<Response<IEnumerable<ProjectDto>>> GetProjectsAsync(
            Guid userId,
            ProjectFilter? filter = null,
            CancellationToken ct = default)
        {
            var projects = await _projectRepository.GetProjectsByUserIdAsync(userId, ct);
            // Применяем фильтр
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                    projects = projects.Where(p => p.Name.Contains(filter.SearchText) ||
                                                   (p.Description?.Contains(filter.SearchText) ?? false));
                if (filter.FromDate.HasValue)
                    projects = projects.Where(p => p.CreatedAt >= filter.FromDate.Value);
                if (filter.ToDate.HasValue)
                    projects = projects.Where(p => p.CreatedAt <= filter.ToDate.Value);
            }
            var dtos = projects.Adapt<List<ProjectDto>>();
            return Response<IEnumerable<ProjectDto>>.Success(dtos);
        }

        public async Task<Response<ProjectDto>> GetProjectByIdAsync(
            Guid id,
            Guid userId,
            CancellationToken ct = default)
        {
            var project = await _projectRepository.GetProjectWithDetailsAsync(id, ct);
            if (project == null)
                return Response<ProjectDto>.Failure("Проект не найден");
            if (project.CreatedById != userId)
                return Response<ProjectDto>.Failure("Нет доступа к этому проекту");
            return Response<ProjectDto>.Success(project.Adapt<ProjectDto>());
        }

        public async Task<Response<ProjectDto>> CreateProjectAsync(
            CreateProjectRequest request,
            Guid userId,
            CancellationToken ct = default)
        {
            var entity = new ProjectEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedById = userId,
                CreatedAt = DateTimeOffset.UtcNow,
                LastModified = DateTimeOffset.UtcNow
            };
            await _projectRepository.AddAsync(entity, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await NotifyProjectChangedAsync("Create", entity, ct);

            return Response<ProjectDto>.Success(entity.Adapt<ProjectDto>());
        }

        public async Task<Response<ProjectDto>> UpdateProjectAsync(
            Guid id,
            UpdateProjectRequest request,
            Guid userId,
            bool isAdmin,
            CancellationToken ct = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, ct);
            if (project == null)
                return Response<ProjectDto>.Failure("Проект не найден");
            if (!isAdmin && project.CreatedById != userId)
                return Response<ProjectDto>.Failure("Нет прав на редактирование");

            project.Name = request.Name;
            project.Description = request.Description;
            project.LastModified = DateTimeOffset.UtcNow;
            _projectRepository.Update(project);
            await _unitOfWork.SaveChangesAsync(ct);

            await NotifyProjectChangedAsync("Update", project, ct);

            return Response<ProjectDto>.Success(project.Adapt<ProjectDto>());
        }

        public async Task<Response<bool>> DeleteProjectAsync(
            Guid id,
            Guid userId,
            bool isAdmin,
            CancellationToken ct = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, ct);
            if (project == null)
                return Response<bool>.Failure("Проект не найден");
            if (!isAdmin && project.CreatedById != userId)
                return Response<bool>.Failure("Нет прав на удаление");

            // Сохраняем DTO для уведомления перед удалением
            var dto = project.Adapt<ProjectDto>();

            _projectRepository.Delete(project);
            await _unitOfWork.SaveChangesAsync(ct);

            await _projectHubContext.Clients.Group($"project_{id}")
                .SendAsync("ProjectChanged", "Delete", dto, ct);

            return Response<bool>.Success(true);
        }
    }
}