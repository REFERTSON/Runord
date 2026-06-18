using Runord.Shared.Base;
using Runord.Shared.DTOs.User;
using Runord.Shared.Filters;

namespace Runord.Shared.Interfaces
{
    public interface IUserService
    {
        // Метод для получения списка пользователей с возможностью фильтрации
        Task<Response<IEnumerable<UserDto>>> GetUsersAsync(
            UserFilter? filter = null,
            CancellationToken ct = default);

        Task<Response<UserDto>> GetUserByIdAsync(Guid id, CancellationToken ct = default);

        //Метод для создания нового пользователя
        Task<Response<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default);

        // Методы для обновления информации о пользователе
        Task<Response<UserDto>> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);
        Task<Response<bool>> UpdateAvatarAsync(Guid userId, UpdateAvatarUserRequest request, CancellationToken ct = default);

        // Метод для удаления пользователя
        Task<Response<bool>> DeleteUserAsync(Guid id, CancellationToken ct = default);

        // Методы для блокировки и разблокировки пользователя
        Task<Response<bool>> BlockUserAsync(Guid userId, CancellationToken ct = default);
        Task<Response<bool>> UnblockUserAsync(Guid userId, CancellationToken ct = default);
    }
}