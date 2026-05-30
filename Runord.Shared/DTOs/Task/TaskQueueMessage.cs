using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Shared.DTOs.Task
{
    public class TaskQueueMessage
    {
        public Guid TaskId { get; set; }
        public string TaskType { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public List<string> InputFilePaths { get; set; } = new();
        public Dictionary<string, string> Parameters { get; set; } = new();
        public Guid OwnerId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class TaskResultMessage
    {
        public Guid TaskId { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> OutputFilePaths { get; set; } = new();
        public double ExecutionTimeSeconds { get; set; }
        public DateTimeOffset CompletedAt { get; set; }
    }
}
