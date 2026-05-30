using Runord.Shared.Base;
using Runord.Shared.DTOs.Auth;

namespace Runord.Shared.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<TokenResponse>> LoginAsync(string email, string password, CancellationToken ct = default);
        Task<Result<bool>> LogoutAsync(Guid userId, CancellationToken ct = default);
        Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
        Task<Result<bool>> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword, CancellationToken ct = default);
        Task<Result<bool>> ForgotPasswordAsync(string email, CancellationToken ct = default);
        Task<Result<bool>> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default);
        Task<Result<bool>> ConfirmEmailAsync(string email, string token, CancellationToken ct = default);
        Task<Result<bool>> ResendConfirmationEmailAsync(string email, CancellationToken ct = default);
    }
}