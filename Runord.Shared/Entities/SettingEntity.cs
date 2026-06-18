using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.Entities
{
    public class SettingEntity : BaseEntity
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;   // храним как строку (сериализованное значение)
        public SettingValueType ValueType { get; set; }     // тип значения
        public string? EnumTypeName { get; set; }           // полное имя enum (только для Enum)
    }
}