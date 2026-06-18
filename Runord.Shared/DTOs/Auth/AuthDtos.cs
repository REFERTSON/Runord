using Runord.Shared.DTOs.User;

namespace Runord.Shared.DTOs.Auth
{
    // DTO запроса на аутентификацию, содержащий email и пароль
    public record LoginRequest(string Email, string Password);

    // DTO запроса на выход из системы, содержащий идентификатор пользователя и refresh token для его инвалидирования
    public record LogoutRequest(Guid UserId, string RefreshToken);

    // DTO для запроса обновления токена, содержащий текущий refresh token
    public record RefreshTokenRequest(string RefreshToken);

    // DTO ответа при успешной аутентификации, содержащий токены и информацию о пользователе
    public record TokenResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAt,
        string RefreshToken,
        DateTime RefreshTokenExpiresAt,
        UserDto User
    );
}