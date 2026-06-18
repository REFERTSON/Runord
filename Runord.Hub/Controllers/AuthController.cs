using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Auth;
using Runord.Shared.Interfaces.Services;
using System.ComponentModel;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<TokenResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Response<TokenResponse>))]
        public async Task<ActionResult<Response<TokenResponse>>> Login(LoginRequest request, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(request.Email, request.Password, ct);
            return !result.IsSuccess ? Unauthorized(result) : Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        [EndpointSummary("Выход из системы (Отзыв токена)")]
        [EndpointDescription("Завершает текущую сессию пользователя и инвалидирует (удаляет) активный Refresh Token в базе данных.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Response<bool>>> Logout(CancellationToken ct)
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<TokenResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Response<TokenResponse>))]
        public async Task<ActionResult<Response<TokenResponse>>> RefreshToken(RefreshTokenRequest request, CancellationToken ct)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken, ct);
            return !result.IsSuccess ? Unauthorized(result) : Ok(result);
        }

        // Вспомогательный метод для извлечения UserId из JWT токена
        private Guid? GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }
}