using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.DTOs.User
{
    // DTO для передачи данных о пользователе
    public record UserDto(
        Guid Id,
        string Email,
        string FullName,
        string AvatarUrl,
        UserRole Role,
        DateTimeOffset? LastLoginTime,
        DateTimeOffset CreatedAt
    ) : BaseDto(Id);

    // DTO для создания нового пользователя
    public record CreateUserRequest(
        string Email,
        string Password,
        string ConfirmPassword,
        string FullName,
        UserRole Role
    );

    // DTO для обновления данных существующего пользователя
    public record UpdateUserRequest(
        string FullName,
        string Email,
        UserRole Role
    );

    // DTO для обновления аватара пользователя
    public record UpdateAvatarUserRequest(
        string AvatarUrl
    );

    public record UserChangedNotification(
        UserAction Action,
        UserDto User
    );
}