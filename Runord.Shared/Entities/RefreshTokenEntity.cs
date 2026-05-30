using Runord.Shared.Base;

namespace Runord.Shared.Entities
{
    public class RefreshTokenEntity : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset? RevokedAt { get; set; }
        public string? ReplacedByTokenId { get; set; }

        public virtual UserEntity User { get; set; } = null!;
    }
}