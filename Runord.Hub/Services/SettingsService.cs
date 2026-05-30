using Microsoft.Extensions.Caching.Memory;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Settings;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IMemoryCache _cache;

        public SettingsService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<Result<SettingDto<object>>> GetSettingAsync(string key, CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(key, out object? value))
                return Task.FromResult(Result<SettingDto<object>>.Success(new SettingDto<object>(key, value!)));
            return Task.FromResult(Result<SettingDto<object>>.Failure($"Настройка '{key}' не найдена"));
        }

        public Task<Result<SettingDto<object>>> UpdateSettingAsync(string key, object value, CancellationToken cancellationToken = default)
        {
            _cache.Set(key, value);
            return Task.FromResult(Result<SettingDto<object>>.Success(new SettingDto<object>(key, value)));
        }

        public Task<Result<IEnumerable<SettingDto<object>>>> GetAllSettingsAsync(CancellationToken cancellationToken = default)
        {
            var keys = new[] { "interface-language", "theme", "auto-start" };
            var result = new List<SettingDto<object>>();
            foreach (var key in keys)
            {
                if (_cache.TryGetValue(key, out object? val))
                    result.Add(new SettingDto<object>(key, val!));
            }
            return Task.FromResult(Result<IEnumerable<SettingDto<object>>>.Success(result));
        }
    }
}