using Runord.Shared.Base;
using Runord.Shared.DTOs.Auth;

namespace Runord.Shared.Interfaces.Services
{
    public interface IAuthService
    {
        // Метод аутентификации пользователя, возвращающий токены доступа и обновления
        Task<Response<TokenResponse>> LoginAsync(string email, string password, CancellationToken ct = default);

        // Метод выхода пользователя, который может включать в себя удаление токенов
        Task<Response<bool>> LogoutAsync(Guid userId, CancellationToken ct = default);

        // Метод для обновления токенов с помощью токена обновления
        Task<Response<TokenResponse>> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    }
}