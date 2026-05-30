using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Auth;
using Runord.Shared.DTOs.User;
using Runord.Shared.Interfaces.Services;
using System.ComponentModel; // Нужен для атрибута [Description]
using System.Security.Claims;

namespace Runord.Hub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Description("Управление аутентификацией, сессиями и безопасностью учетных записей пользователей.")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [AllowAnonymous]
        [HttpPost("login")]
        [EndpointSummary("Аутентификация пользователя")]
        [EndpointDescription("Проверяет учетные данные пользователя (Email/Password) и выдает пару JWT Access Token и Refresh Token.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<TokenResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<TokenResponse>))]
        public async Task<ActionResult<Result<TokenResponse>>> Login(LoginRequest request, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(request.Email, request.Password, ct);
            return !result.IsSuccess ? Unauthorized(result) : Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        [EndpointSummary("Выход из системы (Отзыв токена)")]
        [EndpointDescription("Завершает текущую сессию пользователя и инвалидирует (удаляет) активный Refresh Token в базе данных.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Result<bool>>> Logout(CancellationToken ct)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            var result = await _authService.LogoutAsync(userId.Value, ct);
            return !result.IsSuccess ? BadRequest(result) : Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        [EndpointSummary("Обновление пары токенов (Refresh)")]
        [EndpointDescription("Принимает старый Refresh Token, проверяет его валидность и срок действия, после чего генерирует новую пару Access и Refresh токенов.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<TokenResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<TokenResponse>))]
        public async Task<ActionResult<Result<TokenResponse>>> RefreshToken(RefreshTokenRequest request, CancellationToken ct)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken, ct);
            return !result.IsSuccess ? Unauthorized(result) : Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        [EndpointSummary("Смена пароля в личном кабинете")]
        [EndpointDescription("Позволяет авторизованному пользователю изменить свой текущий пароль на новый. Требуется валидация старого пароля.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<bool>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Result<bool>>> ChangePassword(ChangePasswordRequest request, CancellationToken ct)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            if (userId.Value != request.UserId) return Forbid();
            var result = await _authService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword, ct);
            return !result.IsSuccess ? BadRequest(result) : Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [EndpointSummary("Запрос на восстановление пароля")]
        [EndpointDescription("Инициирует процедуру сброса пароля. Отправляет на указанный Email письмо со специальным токеном восстановления.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
        public async Task<ActionResult<Result<bool>>> ForgotPassword(ForgotPasswordRequest request, CancellationToken ct)
        {
            var result = await _authService.ForgotPasswordAsync(request.Email, ct);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        [EndpointSummary("Сброс пароля по токену")]
        [EndpointDescription("Принимает токен восстановления из письма и устанавливает пользователю новый пароль.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<bool>))]
        public async Task<ActionResult<Result<bool>>> ResetPassword(ResetPasswordRequest request, CancellationToken ct)
        {
            var result = await _authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword, ct);
            return !result.IsSuccess ? BadRequest(result) : Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("confirm-email")]
        [EndpointSummary("Подтверждение Email адреса")]
        [EndpointDescription("Активирует аккаунт пользователя с помощью токена подтверждения, отправленного при регистрации.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<bool>))]
        public async Task<ActionResult<Result<bool>>> ConfirmEmail(ConfirmEmailRequest request, CancellationToken ct)
        {
            var result = await _authService.ConfirmEmailAsync(request.Email, request.ConfirmationToken, ct);
            return !result.IsSuccess ? BadRequest(result) : Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("resend-confirmation")]
        [EndpointSummary("Повторная отправка токена подтверждения")]
        [EndpointDescription("Повторно высылает письмо с токеном активации аккаунта на указанный Email, если предыдущее не дошло.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
        public async Task<ActionResult<Result<bool>>> ResendConfirmation(ResendConfirmationRequest request, CancellationToken ct)
        {
            var result = await _authService.ResendConfirmationEmailAsync(request.Email, ct);
            return Ok(result);
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }
}