using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Settings;
using Runord.Shared.Interfaces;
using System.ComponentModel;
using System.Text.Json;

namespace Runord.Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Description("Управление системными настройками и настройками пользователя.")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet("string/{key}")]
        [EndpointSummary("Получить строковую настройку")]
        [EndpointDescription("Возвращает значение настройки типа string.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<SettingDto<string>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Response<SettingDto<string>>>> GetString(string key)
        {
            var result = await _settingsService.GetStringAsync(key);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("int/{key}")]
        [EndpointSummary("Получить целочисленную настройку")]
        [EndpointDescription("Возвращает значение настройки типа int.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<SettingDto<int>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Response<SettingDto<int>>>> GetInt(string key)
        {
            var result = await _settingsService.GetIntAsync(key);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("bool/{key}")]
        [EndpointSummary("Получить булеву настройку")]
        [EndpointDescription("Возвращает значение настройки типа bool.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<SettingDto<bool>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Response<SettingDto<bool>>>> GetBool(string key)
        {
            var result = await _settingsService.GetBoolAsync(key);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("enum/{key}")]
        [EndpointSummary("Получить настройку-перечисление")]
        [EndpointDescription("Возвращает значение настройки типа enum (как int).")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<SettingDto<int>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Response<SettingDto<int>>>> GetEnum(string key)
        {
            var result = await _settingsService.GetSettingAsync<int>(key);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpPut("{key}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Установить настройку (только Admin)")]
        [EndpointDescription("Устанавливает значение настройки. Тип определяется автоматически.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<SettingDto<object>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Response<SettingDto<object>>>> SetSetting(string key, [FromBody] object value)
        {
            if (value is string stringValue)
            {
                var result = await _settingsService.SetSettingAsync(key, stringValue);
                if (!result.IsSuccess) return BadRequest(result);
                var dto = new SettingDto<object>(result.Data.Key, result.Data.Value);
                return Ok(Response<SettingDto<object>>.Success(dto));
            }

            if (value is int intValue)
            {
                var result = await _settingsService.SetSettingAsync(key, intValue);
                if (!result.IsSuccess) return BadRequest(result);
                var dto = new SettingDto<object>(result.Data.Key, result.Data.Value);
                return Ok(Response<SettingDto<object>>.Success(dto));
            }

            if (value is bool boolValue)
            {
                var result = await _settingsService.SetSettingAsync(key, boolValue);
                if (!result.IsSuccess) return BadRequest(result);
                var dto = new SettingDto<object>(result.Data.Key, result.Data.Value);
                return Ok(Response<SettingDto<object>>.Success(dto));
            }

            if (value is JsonElement elem)
            {
                switch (elem.ValueKind)
                {
                    case JsonValueKind.String:
                        var strResult = await _settingsService.SetSettingAsync(key, elem.GetString()!);
                        if (!strResult.IsSuccess) return BadRequest(strResult);
                        return Ok(Response<SettingDto<object>>.Success(new SettingDto<object>(key, strResult.Data.Value)));
                    case JsonValueKind.Number:
                        if (elem.TryGetInt32(out int num))
                        {
                            var numResult = await _settingsService.SetSettingAsync(key, num);
                            if (!numResult.IsSuccess) return BadRequest(numResult);
                            return Ok(Response<SettingDto<object>>.Success(new SettingDto<object>(key, numResult.Data.Value)));
                        }
                        return BadRequest(Response<SettingDto<object>>.Failure("Число не в диапазоне int"));
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        var boolResult = await _settingsService.SetSettingAsync(key, elem.GetBoolean());
                        if (!boolResult.IsSuccess) return BadRequest(boolResult);
                        return Ok(Response<SettingDto<object>>.Success(new SettingDto<object>(key, boolResult.Data.Value)));
                    default:
                        return BadRequest(Response<SettingDto<object>>.Failure("Неподдерживаемый тип JSON"));
                }
            }

            return BadRequest(Response<SettingDto<object>>.Failure("Неподдерживаемый тип значения"));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Получить все настройки (только Admin)")]
        [EndpointDescription("Возвращает список всех системных настроек.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<IEnumerable<SettingInfoDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Response<IEnumerable<SettingInfoDto>>>> GetAll()
        {
            var result = await _settingsService.GetAllSettingsAsync();
            return Ok(result);
        }
    }
}