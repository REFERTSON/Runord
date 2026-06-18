using Runord.Shared.Base;
using Runord.Shared.DTOs.Settings;

namespace Runord.Shared.Interfaces
{
    public interface ISettingsService
    {
        // Generic получение
        Task<Response<SettingDto<T>>> GetSettingAsync<T>(string key, CancellationToken ct = default) where T : notnull;
        // Generic установка
        Task<Response<SettingDto<T>>> SetSettingAsync<T>(string key, T value, CancellationToken ct = default) where T : notnull;
        // Получение всех (только для админа)
        Task<Response<IEnumerable<SettingInfoDto>>> GetAllSettingsAsync(CancellationToken ct = default);
        // Удобные методы для простых типов
        Task<Response<SettingDto<string>>> GetStringAsync(string key, CancellationToken ct = default);
        Task<Response<SettingDto<int>>> GetIntAsync(string key, CancellationToken ct = default);
        Task<Response<SettingDto<bool>>> GetBoolAsync(string key, CancellationToken ct = default);
        Task<Response<SettingDto<TEnum>>> GetEnumAsync<TEnum>(string key, CancellationToken ct = default) where TEnum : struct, Enum;
    }
}