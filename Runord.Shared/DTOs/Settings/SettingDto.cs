namespace Runord.Shared.DTOs.Settings
{
    // Generic DTO для одного значения
    public record SettingDto<T>(string Key, T Value);

    // Для списка всех настроек (с информацией о типе)
    public record SettingInfoDto(string Key, object Value, string ValueType);
}