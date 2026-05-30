namespace Runord.Shared.Base
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset LastModified { get; set; }
    }
}
