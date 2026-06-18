using Runord.Shared.Base;

namespace Runord.Shared.Entities
{
    // Сущность для хранения информации о refresh токене.
    public class RefreshTokenEntity : BaseEntity
    {
        // ID пользователя, которому принадлежит этот токен.
        public Guid UserId { get; set; }
        public virtual UserEntity User { get; set; } = null!;

        // Сам refresh токен.
        public string Token { get; set; } = string.Empty;

        // Дата и время, когда этот refresh токен истекает и становится недействительным.
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset? RevokedAt { get; set; }
    }
}