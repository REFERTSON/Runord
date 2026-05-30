using Runord.Shared.DTOs.User;

namespace Runord.Shared.DTOs.Auth
{
    public record LoginRequest(string Email, string Password);

    public record TokenResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAt,
        string RefreshToken,
        DateTime RefreshTokenExpiresAt,
        UserDto User
    );

    public record RefreshTokenRequest(string RefreshToken);
    public record LogoutRequest(Guid UserId, string RefreshToken);
    public record ForgotPasswordRequest(string Email);
    public record ResetPasswordRequest(string Email, string Token, string NewPassword);
    public record ConfirmEmailRequest(string Email, string ConfirmationToken);
    public record ResendConfirmationRequest(string Email);
}