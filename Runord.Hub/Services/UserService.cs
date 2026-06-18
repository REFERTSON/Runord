using Mapster;
using Microsoft.AspNetCore.SignalR;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Hub.Hubs;
using Runord.Shared.Base;
using Runord.Shared.DTOs.User;
using Runord.Shared.Entities;
using Runord.Shared.Enums;
using Runord.Shared.Filters;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<UsersHub> _usersHubContext;

        public UserService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IHubContext<UsersHub> usersHubContext)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _usersHubContext = usersHubContext;
        }

        private async Task NotifyUsersAsync(UserAction action, UserEntity user, CancellationToken ct = default)
        {
            var userDto = user.Adapt<UserDto>();
            var notification = new UserChangedNotification(action, userDto);
            await _usersHubContext.Clients.Group("users_updates")
                .SendAsync("UserChanged", notification, ct);
        }

        public async Task<Response<IEnumerable<UserDto>>> GetUsersAsync(UserFilter? filter, CancellationToken ct = default)
        {
            var users = await _userRepository.GetUsersAsync(filter, ct);
            return Response<IEnumerable<UserDto>>.Success(users.Adapt<List<UserDto>>());
        }

        public async Task<Response<UserDto>> GetUserByIdAsync(Guid id, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            if (user == null)
                return Response<UserDto>.Failure("Пользователь не найден");
            return Response<UserDto>.Success(user.Adapt<UserDto>());
        }

        public async Task<Response<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
        {
            if (request.Password != request.ConfirmPassword)
                return Response<UserDto>.Failure("Пароли не совпадают");

            if (!await _userRepository.IsEmailUniqueAsync(request.Email, null, ct))
                return Response<UserDto>.Failure("Пользователь с таким email уже существует");

            var entity = request.Adapt<UserEntity>();
            entity.Id = Guid.NewGuid();
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            entity.CreatedAt = DateTimeOffset.UtcNow;
            entity.LastModified = DateTimeOffset.UtcNow;
            entity.IsBlocked = false;

            await _userRepository.AddAsync(entity, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await NotifyUsersAsync(UserAction.Create, entity, ct);

            return Response<UserDto>.Success(entity.Adapt<UserDto>());
        }

        public async Task<Response<UserDto>> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            if (user == null)
                return Response<UserDto>.Failure("Пользователь не найден");

            if (request.FullName != user.FullName)
                user.FullName = request.FullName;

            if (request.Email != user.Email)
            {
                if (!await _userRepository.IsEmailUniqueAsync(request.Email, id, ct))
                    return Response<UserDto>.Failure("Пользователь с таким email уже существует");
                user.Email = request.Email;
            }

            if (request.Role != user.Role)
                user.Role = request.Role;

            user.LastModified = DateTimeOffset.UtcNow;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            await NotifyUsersAsync(UserAction.Update, user, ct);

            return Response<UserDto>.Success(user.Adapt<UserDto>());
        }

        public async Task<Response<bool>> DeleteUserAsync(Guid id, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            if (user == null)
                return Response<bool>.Failure("Пользователь не найден");

            var userDto = user.Adapt<UserDto>();

            // ИСПРАВЛЕНО: Жесткое удаление токенов при удалении учетной записи
            await _refreshTokenRepository.DeleteAllForUserAsync(id, ct);

            _userRepository.Delete(user);
            await _unitOfWork.SaveChangesAsync(ct);

            await _usersHubContext.Clients.Group("users_updates")
                .SendAsync("UserChanged", new UserChangedNotification(UserAction.Delete, userDto), ct);

            return Response<bool>.Success(true);
        }

        public async Task<Response<bool>> UpdateAvatarAsync(Guid userId, UpdateAvatarUserRequest request, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
                return Response<bool>.Failure("Пользователь не найден");

            user.AvatarUrl = request.AvatarUrl;
            user.LastModified = DateTimeOffset.UtcNow;

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            await NotifyUsersAsync(UserAction.AvatarUpdate, user, ct);

            return Response<bool>.Success(true);
        }

        public async Task<Response<bool>> BlockUserAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
                return Response<bool>.Failure("Пользователь не найден");

            user.IsBlocked = true;
            user.LastModified = DateTimeOffset.UtcNow;

            _userRepository.Update(user);

            // ИСПРАВЛЕНО: Удаляем все токены заблокированного пользователя, чтобы его сессия сразу прервалась
            await _refreshTokenRepository.DeleteAllForUserAsync(userId, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            await NotifyUsersAsync(UserAction.Block, user, ct);

            return Response<bool>.Success(true);
        }

        public async Task<Response<bool>> UnblockUserAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
                return Response<bool>.Failure("Пользователь не найден");

            user.IsBlocked = false;
            user.LastModified = DateTimeOffset.UtcNow;

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            await NotifyUsersAsync(UserAction.Unblock, user, ct);

            return Response<bool>.Success(true);
        }
    }
}