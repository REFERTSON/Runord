using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.Entities
{
    // Cущность пользователя, представляющая данные о пользователе в системе.
    public class UserEntity : BaseEntity
    {
        // Email пользователя, используемый для входа.
        public string Email { get; set; } = string.Empty;
        // Хэш пароля для безопасного хранения пароля пользователя.
        public string PasswordHash { get; set; } = string.Empty;
        // Полное имя пользователя для отображения в интерфейсе.
        public string FullName { get; set; } = string.Empty;
        // Ссылка на аватар пользователя, используемая для отображения его изображения в интерфейсе.
        public string? AvatarUrl { get; set; }
        // Роль пользователя, определяющая его права доступа в системе.
        public UserRole Role { get; set; }
        // Статус онлайн/офлайн для отображения текущего состояния пользователя.
        public bool IsOnline { get; set; }
        // Время последнего входа пользователя в систему для отслеживания активности.
        public DateTimeOffset? LastLoginTime { get; set; }

        // Флаг блокировки пользователя, указывающий, может ли он входить в систему.
        public bool IsBlocked { get; set; }

        // Навигационные свойства
        public virtual ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();
        public virtual ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
        public virtual ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = new List<RefreshTokenEntity>();
    }
}