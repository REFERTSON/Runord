using Runord.Shared.Base;
using Runord.Shared.DTOs.Settings;

namespace Runord.Shared.Interfaces
{
    public interface ISettingsService
    {
        Task<Result<SettingDto<object>>> GetSettingAsync(string key, CancellationToken cancellationToken = default);
        Task<Result<SettingDto<object>>> UpdateSettingAsync(string key, object value, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<SettingDto<object>>>> GetAllSettingsAsync(CancellationToken cancellationToken = default);
    }
}