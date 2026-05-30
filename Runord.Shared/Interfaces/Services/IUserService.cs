using Runord.Shared.Base;
using Runord.Shared.DTOs.User;

namespace Runord.Shared.Interfaces
{
    public interface IUserService
    {
        Task<Result<PagedResponse<UserDto>>> GetUsersAsync(int page, int pageSize, CancellationToken ct = default);
        Task<Result<UserDto>> GetUserByIdAsync(Guid id, CancellationToken ct = default);
        Task<Result<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default);
        Task<Result<UserDto>> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);
        Task<Result<bool>> DeleteUserAsync(Guid id, CancellationToken ct = default);
        Task<Result<bool>> ChangeEmailAsync(Guid userId, ChangeEmailRequest request, CancellationToken ct = default);
        Task<Result<bool>> UpdateAvatarAsync(Guid userId, UpdateAvatarRequest request, CancellationToken ct = default);
        Task<Result<bool>> BlockUserAsync(Guid userId, BlockUserRequest request, CancellationToken ct = default);
        Task<Result<bool>> UnblockUserAsync(Guid userId, CancellationToken ct = default);
    }
}