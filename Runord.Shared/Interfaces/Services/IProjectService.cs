using Runord.Shared.Base;
using Runord.Shared.DTOs.Project;
using Runord.Shared.Filters;

namespace Runord.Shared.Interfaces
{
    public interface IProjectService
    {
        // Получение проектов текущего пользователя (с фильтром)
        Task<Response<IEnumerable<ProjectDto>>> GetProjectsAsync(
            Guid userId,
            ProjectFilter? filter = null,
            CancellationToken ct = default);

        // Получение одного проекта (только если пользователь владелец)
        Task<Response<ProjectDto>> GetProjectByIdAsync(
            Guid id,
            Guid userId,
            CancellationToken ct = default);

        // Создание проекта (userId – создатель)
        Task<Response<ProjectDto>> CreateProjectAsync(
            CreateProjectRequest request,
            Guid userId,
            CancellationToken ct = default);

        // Обновление проекта (проверка прав внутри)
        Task<Response<ProjectDto>> UpdateProjectAsync(
            Guid id,
            UpdateProjectRequest request,
            Guid userId,
            bool isAdmin,
            CancellationToken ct = default);

        // Удаление проекта (проверка прав)
        Task<Response<bool>> DeleteProjectAsync(
            Guid id,
            Guid userId,
            bool isAdmin,
            CancellationToken ct = default);
    }
}