using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.Entities
{
    public class UserEntity : BaseEntity
    {
        // Основные поля
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsOnline { get; set; }
        public DateTimeOffset? LastLoginTime { get; set; }

        // Подтверждение email
        public bool EmailConfirmed { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public DateTimeOffset? EmailConfirmationTokenExpiresAt { get; set; }

        // Сброс пароля
        public string? PasswordResetToken { get; set; }
        public DateTimeOffset? PasswordResetTokenExpiresAt { get; set; }

        // Блокировка (опционально)
        public bool IsBlocked { get; set; }
        public string? BlockReason { get; set; }

        // Навигационные свойства
        public virtual ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();
        public virtual ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
        public virtual ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = new List<RefreshTokenEntity>();
    }
}