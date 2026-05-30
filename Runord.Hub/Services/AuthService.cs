using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Auth;
using Runord.Shared.DTOs.User;
using Runord.Shared.Entities;
using Runord.Shared.Interfaces;
using Runord.Shared.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Runord.Hub.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<Result<TokenResponse>> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, ct);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Result<TokenResponse>.Failure("Неверный email или пароль");

            if (!user.EmailConfirmed)
                return Result<TokenResponse>.Failure("Email не подтверждён. Проверьте почту.");
            if (user.IsBlocked)
                return Result<TokenResponse>.Failure("Учётная запись заблокирована.");

            var accessToken = GenerateJwtToken(user);
            var refreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id, ct);

            user.LastLoginTime = DateTimeOffset.UtcNow;
            user.IsOnline = true;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<TokenResponse>.Success(CreateTokenResponse(user, accessToken, refreshToken));
        }

        public async Task<Result<bool>> LogoutAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
                return Result<bool>.Failure("Пользователь не найден");

            user.IsOnline = false;
            _userRepository.Update(user);
            await _refreshTokenRepository.RevokeAllForUserAsync(userId, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, ct);
            if (storedToken == null || storedToken.RevokedAt != null)
                return Result<TokenResponse>.Failure("Недействительный refresh token");
            if (storedToken.ExpiresAt < DateTimeOffset.UtcNow)
                return Result<TokenResponse>.Failure("Refresh token истёк");

            var user = await _userRepository.GetByIdAsync(storedToken.UserId, ct);
            if (user == null)
                return Result<TokenResponse>.Failure("Пользователь не найден");

            storedToken.RevokedAt = DateTimeOffset.UtcNow;
            _refreshTokenRepository.Update(storedToken);

            var newRefreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id, ct);
            var newAccessToken = GenerateJwtToken(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<TokenResponse>.Success(CreateTokenResponse(user, newAccessToken, newRefreshToken));
        }

        public async Task<Result<bool>> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
                return Result<bool>.Failure("Пользователь не найден");
            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                return Result<bool>.Failure("Неверный старый пароль");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.LastModified = DateTimeOffset.UtcNow;
            _userRepository.Update(user);
            await _refreshTokenRepository.RevokeAllForUserAsync(userId, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ForgotPasswordAsync(string email, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, ct);
            if (user == null)
                return Result<bool>.Success(true); // не раскрываем существование

            user.PasswordResetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.PasswordResetTokenExpiresAt = DateTimeOffset.UtcNow.AddHours(1);
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);
            // TODO: отправить email
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, ct);
            if (user == null || user.PasswordResetToken != token || user.PasswordResetTokenExpiresAt < DateTimeOffset.UtcNow)
                return Result<bool>.Failure("Недействительная или истёкшая ссылка сброса пароля");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiresAt = null;
            _userRepository.Update(user);
            await _refreshTokenRepository.RevokeAllForUserAsync(user.Id, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ConfirmEmailAsync(string email, string token, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, ct);
            if (user == null || user.EmailConfirmationToken != token || user.EmailConfirmationTokenExpiresAt < DateTimeOffset.UtcNow)
                return Result<bool>.Failure("Недействительная или истёкшая ссылка подтверждения");

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpiresAt = null;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ResendConfirmationEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, ct);
            if (user == null || user.EmailConfirmed)
                return Result<bool>.Success(true);

            user.EmailConfirmationToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.EmailConfirmationTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(1);
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);
            // TODO: отправить email
            return Result<bool>.Success(true);
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("FullName", user.FullName)
            };
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(Guid userId, CancellationToken ct)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshToken = new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
                CreatedAt = DateTimeOffset.UtcNow,
                LastModified = DateTimeOffset.UtcNow
            };
            await _refreshTokenRepository.AddAsync(refreshToken, ct);
            return token;
        }

        private TokenResponse CreateTokenResponse(UserEntity user, string accessToken, string refreshToken)
        {
            var userDto = new UserDto(
                user.Id, user.Email, user.FullName, "",
                user.Group, user.Role, user.LastLoginTime, user.CreatedAt);
            return new TokenResponse(
                AccessToken: accessToken,
                AccessTokenExpiresAt: DateTime.UtcNow.AddHours(1),
                RefreshToken: refreshToken,
                RefreshTokenExpiresAt: DateTime.UtcNow.AddDays(7),
                User: userDto);
        }
    }
}