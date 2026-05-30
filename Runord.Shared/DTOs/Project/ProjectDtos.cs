using Runord.Shared.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Shared.DTOs.Project
{
    public record ProjectDto(
        Guid Id,
        string Name,
        string Description,
        int TaskCount,
        string CreatorName,
        DateTimeOffset CreatedAt
    ) : BaseDto(Id);

    public record CreateProjectRequest(string Name, string Description);
    public record UpdateProjectRequest(Guid Id, string Name, string Description);
    public record DeleteProjectRequest(Guid Id);
}
