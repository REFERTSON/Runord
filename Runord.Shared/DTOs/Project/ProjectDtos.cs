using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.DTOs.Project
{
    // DTO для передачи данных о проекте
    public record ProjectDto(
        Guid Id,
        string Name,
        string Description,
        int TaskCount,
        DateTimeOffset CreatedAt
    ) : BaseDto(Id);

    // DTO для создания, обновления и удаления проекта
    public record CreateProjectRequest(string Name, string Description);
    public record UpdateProjectRequest(string Name, string Description);
    public record ProjectChangedNotification(
        ProjectAction Action,
        ProjectDto Project
    );
}
