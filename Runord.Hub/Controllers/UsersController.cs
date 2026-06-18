using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting; // Добавлено для IWebHostEnvironment
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.User;
using Runord.Shared.Interfaces;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace Runord.Hub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Description("Управление пользователями: профиль, список, блокировка, аватар.")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment; // Добавлено для работы с путями сервера

        public UsersController(IUserService userService, IWebHostEnvironment environment)
        {
            _userService = userService;
            _environment = environment;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [EndpointSummary("Получить список всех пользователей (только Admin)")]
        [EndpointDescription("Возвращает список всех зарегистрированных пользователей.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<IEnumerable<UserDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Response<IEnumerable<UserDto>>>> GetUsers()
        {
            var result = await _userService.GetUsersAsync();
            return Ok(result);
        }

        [HttpGet("me")]
        [EndpointSummary("Получить профиль текущего пользователя")]
        [EndpointDescription("Возвращает данные авторизованного пользователя.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<UserDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Response<UserDto>>> GetCurrentUser()
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();

            var result = await _userService.GetUserByIdAsync(userId.Value);
            if (!result.IsSuccess) return NotFound(result);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [EndpointSummary("Получить пользователя по ID")]
        [EndpointDescription("Возвращает данные пользователя. Доступно администратору или самому пользователю.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<UserDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Response<UserDto>>> GetUser(Guid id)
        {
            var currentUserId = GetUserId();
            if (!currentUserId.HasValue) return Unauthorized();
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && currentUserId != id) return Forbid();
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [EndpointSummary("Создать пользователя (только Admin)")]
        [EndpointDescription("Создаёт нового пользователя с указанными данными.")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Response<UserDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Response<UserDto>>> CreateUser(CreateUserRequest request)
        {
            var result = await _userService.CreateUserAsync(request);
            if (!result.IsSuccess) return BadRequest(result);
            return CreatedAtAction(nameof(GetUser), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [EndpointSummary("Обновить данные пользователя")]
        [EndpointDescription("Обновляет профиль. Доступно администратору или самому пользователю.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<UserDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Response<UserDto>>> UpdateUser(Guid id, UpdateUserRequest request)
        {
            var currentUserId = GetUserId();
            if (!currentUserId.HasValue) return Unauthorized();
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && currentUserId != id) return Forbid();

            var result = await _userService.UpdateUserAsync(id, request);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        [EndpointSummary("Удалить пользователя (только Admin)")]
        [EndpointDescription("Безвозвратно удаляет пользователя из системы.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Response<bool>>> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpPatch("me/avatar/upload")]
        [Consumes("multipart/form-data")]
        [EndpointSummary("Загрузить аватар")]
        [EndpointDescription("Загружает изображение аватара для текущего пользователя.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Response<bool>>> UploadAvatar([FromForm(Name = "file")] IFormFile file)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest(Response<bool>.Failure("Файл не выбран"));

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                return BadRequest(Response<bool>.Failure("Неподдерживаемый формат изображения"));

            // ИСПРАВЛЕНО: Безопасное получение пути к wwwroot через окружение среды
            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrEmpty(webRootPath))
            {
                // Фолбек на случай, если WebRootPath не инициализирован в тестовой среде
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var uploadPath = Path.Combine(webRootPath, "avatars");

            // ИСПРАВЛЕНО: Безопасное имя файла (используем UTC время, чтобы избежать проблем с локалями)
            var fileName = $"{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{ext}";
            var filePath = Path.Combine(uploadPath, fileName);

            try
            {
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                // ИСПРАВЛЕНО: Перехватываем ошибки работы с файловой системой, чтобы сервер не падал
                return StatusCode(StatusCodes.Status500InternalServerError,
                    Response<bool>.Failure($"Критическая ошибка диска при сохранении аватара: {ex.Message}"));
            }

            var avatarUrl = $"/avatars/{fileName}";
            var result = await _userService.UpdateAvatarAsync(userId.Value, new UpdateAvatarUserRequest(avatarUrl));
            if (!result.IsSuccess) return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:guid}/block")]
        [EndpointSummary("Заблокировать пользователя (только Admin)")]
        [EndpointDescription("Блокирует учётную запись пользователя.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Response<bool>>> BlockUser(Guid id)
        {
            var result = await _userService.BlockUserAsync(id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:guid}/unblock")]
        [EndpointSummary("Разблокировать пользователя (только Admin)")]
        [EndpointDescription("Снимает блокировку с учётной записи пользователя.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Response<bool>>> UnblockUser(Guid id)
        {
            var result = await _userService.UnblockUserAsync(id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }
}