using Runord.Shared.Base;

namespace Runord.Shared.Entities
{
    public class TaskFileEntity : BaseEntity
    {
        public Guid TaskId { get; set; }
        public virtual TaskEntity? Task { get; set; }

        public string Name { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public string Md5Hash { get; set; } = string.Empty;
        public bool IsResult { get; set; } = false;   // true – выходной файл, false – входной
    }
}