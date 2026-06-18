// Runord.Hub/Services/SettingsService.cs
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Hub.Hubs;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Settings;
using Runord.Shared.Entities;
using Runord.Shared.Enums;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<SettingHub> _hubContext;

        public SettingsService(
            ISettingRepository repository,
            IMemoryCache cache,
            IUnitOfWork unitOfWork,
            IHubContext<SettingHub> hubContext)
        {
            _repository = repository;
            _cache = cache;
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        #region Generic

        public async Task<Response<SettingDto<T>>> GetSettingAsync<T>(string key, CancellationToken ct = default) where T : notnull
        {
            if (_cache.TryGetValue(key, out object? cached) && cached is T typed)
                return Response<SettingDto<T>>.Success(new SettingDto<T>(key, typed));

            var entity = await _repository.GetByKeyAsync(key, ct);
            if (entity == null)
                return Response<SettingDto<T>>.Failure($"Настройка '{key}' не найдена");

            try
            {
                var value = DeserializeValue<T>(entity);
                _cache.Set(key, value);
                return Response<SettingDto<T>>.Success(new SettingDto<T>(key, value));
            }
            catch (Exception ex)
            {
                return Response<SettingDto<T>>.Failure($"Ошибка преобразования: {ex.Message}");
            }
        }

        public async Task<Response<SettingDto<T>>> SetSettingAsync<T>(string key, T value, CancellationToken ct = default) where T : notnull
        {
            var entity = await _repository.GetByKeyAsync(key, ct);
            bool isNew = entity == null;
            if (isNew)
            {
                entity = new SettingEntity
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    CreatedAt = DateTimeOffset.UtcNow
                };
            }

            // Сериализуем значение
            (string serialized, SettingValueType valType, string? enumTypeName) = SerializeValue(value);
            entity.Value = serialized;
            entity.ValueType = valType;
            entity.EnumTypeName = enumTypeName;
            entity.LastModified = DateTimeOffset.UtcNow;

            if (isNew)
                await _repository.AddAsync(entity, ct);
            else
                _repository.Update(entity);

            await _unitOfWork.SaveChangesAsync(ct);
            _cache.Set(key, value);

            // Уведомляем всех подписанных клиентов
            await _hubContext.Clients.Group("settings_updates")
                .SendAsync("SettingChanged", key, value, ct);

            return Response<SettingDto<T>>.Success(new SettingDto<T>(key, value));
        }

        public async Task<Response<IEnumerable<SettingInfoDto>>> GetAllSettingsAsync(CancellationToken ct = default)
        {
            var entities = await _repository.GetAllAsync(ct);
            var result = new List<SettingInfoDto>();
            foreach (var e in entities)
            {
                var obj = DeserializeValueToObject(e);
                if (obj != null)
                    result.Add(new SettingInfoDto(e.Key, obj, e.ValueType.ToString()));
            }
            return Response<IEnumerable<SettingInfoDto>>.Success(result);
        }

        #endregion

        #region Удобные методы

        public async Task<Response<SettingDto<string>>> GetStringAsync(string key, CancellationToken ct = default)
            => await GetSettingAsync<string>(key, ct);

        public async Task<Response<SettingDto<int>>> GetIntAsync(string key, CancellationToken ct = default)
            => await GetSettingAsync<int>(key, ct);

        public async Task<Response<SettingDto<bool>>> GetBoolAsync(string key, CancellationToken ct = default)
            => await GetSettingAsync<bool>(key, ct);

        public async Task<Response<SettingDto<TEnum>>> GetEnumAsync<TEnum>(string key, CancellationToken ct = default) where TEnum : struct, Enum
            => await GetSettingAsync<TEnum>(key, ct);

        #endregion

        #region Приватные сериализаторы

        private (string serialized, SettingValueType type, string? enumTypeName) SerializeValue<T>(T value)
        {
            var t = typeof(T);
            if (t == typeof(string))
                return (value.ToString()!, SettingValueType.String, null);
            if (t == typeof(int))
                return (Convert.ToInt32(value).ToString(), SettingValueType.Integer, null);
            if (t == typeof(long))
                return (Convert.ToInt64(value).ToString(), SettingValueType.Integer, null);
            if (t == typeof(short))
                return (Convert.ToInt16(value).ToString(), SettingValueType.Integer, null);
            if (t == typeof(byte))
                return (Convert.ToByte(value).ToString(), SettingValueType.Integer, null);
            if (t == typeof(bool))
                return (value.ToString()!.ToLower(), SettingValueType.Boolean, null);
            if (t.IsEnum)
                return (Convert.ToInt64(value).ToString(), SettingValueType.Enum, t.AssemblyQualifiedName);
            throw new NotSupportedException($"Тип {t.Name} не поддерживается для настроек");
        }

        private T DeserializeValue<T>(SettingEntity entity) where T : notnull
        {
            if (entity.ValueType == SettingValueType.String && typeof(T) == typeof(string))
                return (T)(object)entity.Value;
            if (entity.ValueType == SettingValueType.Integer && typeof(T) == typeof(int))
                return (T)(object)int.Parse(entity.Value);
            if (entity.ValueType == SettingValueType.Integer && typeof(T) == typeof(long))
                return (T)(object)long.Parse(entity.Value);
            if (entity.ValueType == SettingValueType.Boolean && typeof(T) == typeof(bool))
                return (T)(object)bool.Parse(entity.Value);
            if (entity.ValueType == SettingValueType.Enum && typeof(T).IsEnum)
                return (T)Enum.ToObject(typeof(T), int.Parse(entity.Value));
            throw new InvalidOperationException($"Не удалось преобразовать '{entity.Value}' в {typeof(T).Name}");
        }

        private object? DeserializeValueToObject(SettingEntity entity)
        {
            return entity.ValueType switch
            {
                SettingValueType.String => entity.Value,
                SettingValueType.Integer => int.Parse(entity.Value),
                SettingValueType.Boolean => bool.Parse(entity.Value),
                SettingValueType.Enum when entity.EnumTypeName != null =>
                    Enum.ToObject(Type.GetType(entity.EnumTypeName)!, int.Parse(entity.Value)),
                _ => null
            };
        }

        #endregion
    }
}