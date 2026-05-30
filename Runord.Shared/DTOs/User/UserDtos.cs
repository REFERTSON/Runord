using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.DTOs.User
{
    public record UserDto(
        Guid Id,
        string Email,
        string FullName,
        string AvatarUrl,
        string Group,
        UserRole Role,
        DateTimeOffset? LastLoginTime,
        DateTimeOffset CreatedAt
    ) : BaseDto(Id);

    public record CreateUserRequest(
        string Email,
        string Password,
        string ConfirmPassword,
        string FullName,
        string Group,
        UserRole Role
    );

    public record UpdateUserRequest(
        Guid Id,
        string FullName,
        string Group,
        UserRole Role
    );

    public record DeleteUserRequest(Guid Id);
    public record ChangePasswordRequest(Guid UserId, string CurrentPassword, string NewPassword);
    public record ChangeEmailRequest(Guid UserId, string NewEmail, string Password);
    public record UpdateAvatarRequest(Guid UserId, string AvatarUrl);
    public record BlockUserRequest(Guid UserId, string Reason);
    public record UnblockUserRequest(Guid UserId);
}