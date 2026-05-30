using Mapster;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Shared.Base;
using Runord.Shared.DTOs.User;
using Runord.Shared.Entities;
using Runord.Shared.Interfaces;
using System.Security.Cryptography;

namespace Runord.Hub.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResponse<UserDto>>> GetUsersAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var paged = await _userRepository.GetPagedUsersAsync(page, pageSize, ct);
            var dtos = paged.Items.Adapt<List<UserDto>>();
            var response = new PagedResponse<UserDto>
            {
                Items = dtos,
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
            return Result<PagedResponse<UserDto>>.Success(response);
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(Guid id, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            if (user == null)
                return Result<UserDto>.Failure("Пользователь не найден");
            return Result<UserDto>.Success(user.Adapt<UserDto>());
        }

        public async Task<Result<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
        {
            if (request.Password != request.ConfirmPassword)
                return Result<UserDto>.Failure("Пароли не совпадают");
            if (!await _userRepository.IsEmailUniqueAsync(request.Email, null, ct))
                return Result<UserDto>.Failure("Пользователь с таким email уже существует");

            var entity = request.Adapt<UserEntity>();
            entity.Id = Guid.NewGuid();
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            entity.CreatedAt = DateTimeOffset.UtcNow;
            entity.LastModified = DateTimeOffset.UtcNow;
            entity.EmailConfirmed = false;
            entity.EmailConfirmationToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            entity.EmailConfirmationTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(1);
            entity.IsBlocked = false;

            await _userRepository.AddAsync(entity, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            // TODO: отправить email подтверждения
            return Result<UserDto>.Success(entity.Adapt<UserDto>());
        }

        public async Task<Result<UserDto>> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            if (user == null)
                return Result<UserDto>.Failure("Пользователь не найден");

            user.FullName = request.FullName;
            user.Group = request.Group;
            user.Role = request.Role;
            user.LastModified = DateTimeOffset.UtcNow;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<UserDto>.Success(user.Adapt<UserDto>());
        }

        public async Task<Result<bool>> DeleteUserAsync(Guid id, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            if (user == null)
                return Result<bool>.Failure("Пользователь не найден");

            await _refreshTokenRepository.RevokeAllForUserAsync(id, ct);
            _userRepository.Delete(user);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ChangeEmailAsync(Guid userId, ChangeEmailRequest request, CancellationToken ct = default)
        {
            if (userId != request.UserId)
                return Result<bool>.Failure("Доступ запрещён");

            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
                return Result<bool>.Failure("Пользователь не найден");
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Result<bool>.Failure("Неверный пароль");
            if (!await _userRepository.IsEmailUniqueAsync(request.NewEmail, userId, ct))
                return Result<bool>.Failure("Новый email уже используется");

            user.Email = request.NewEmail;
            user.EmailConfirmed = false;
            user.EmailConfirmationToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.EmailConfirmationTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(1);
            user.LastModified = DateTimeOffset.UtcNow;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);
            // TODO: отправить письмо
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdateAvatarAsync(Guid userId, UpdateAvatarRequest request, CancellationToken ct = default)
        {
            if (userId != request.UserId)
                return Result<bool>.Failure("Доступ запрещён");
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null) return Result<bool>.Failure("Пользователь не найден");
            // Здесь логика сохранения файла
            // user.AvatarUrl = request.AvatarUrl;
            user.LastModified = DateTimeOffset.UtcNow;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> BlockUserAsync(Guid userId, BlockUserRequest request, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, ct);
            if (user == null) return Result<bool>.Failure("Пользователь не найден");
            user.IsBlocked = true;
            user.BlockReason = request.Reason;
            user.LastModified = DateTimeOffset.UtcNow;
            _userRepository.Update(user);
            await _refreshTokenRepository.RevokeAllForUserAsync(request.UserId, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UnblockUserAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null) return Result<bool>.Failure("Пользователь не найден");
            user.IsBlocked = false;
            user.BlockReason = null;
            user.LastModified = DateTimeOffset.UtcNow;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }
    }
}