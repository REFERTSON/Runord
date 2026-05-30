using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Settings;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Все эндпоинты требуют авторизации
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        // ============================================
        // 1. Получение конкретной настройки
        // ============================================
        [HttpGet("{key}")]
        public async Task<ActionResult<Result<SettingDto<object>>>> GetSetting(string key)
        {
            // Разрешено всем аутентифицированным пользователям (читать настройки)
            var result = await _settingsService.GetSettingAsync(key);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        // ============================================
        // 2. Обновление конкретной настройки
        // ============================================
        [HttpPut("{key}")]
        [Authorize(Roles = "Admin")] // Только Админ может менять настройки (если они глобальные)
        public async Task<ActionResult<Result<SettingDto<object>>>> UpdateSetting(string key, [FromBody] SettingDto<object> dto)
        {
            if (key != dto.Key)
                return BadRequest(Result<SettingDto<object>>.Failure("Ключ в URL и теле запроса не совпадают."));

            var result = await _settingsService.UpdateSettingAsync(key, dto.Value);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ============================================
        // 3. Дополнительный эндпоинт: все настройки (если нужно)
        // ============================================
        [HttpGet]
        public async Task<ActionResult<Result<IEnumerable<SettingDto<object>>>>> GetAllSettings()
        {
            var result = await _settingsService.GetAllSettingsAsync();
            return Ok(result);
        }
    }
}
