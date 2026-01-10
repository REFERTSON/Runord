using Newtonsoft.Json;
using Runord.Client.App.Storage.Entities.Base;
using Runord.Client.Shared.Enums;
using Runord.Client.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runord.Client.App.Storage.Entities
{
    public class TaskItem : Entity
    {
        public Guid UUID { get; init; } = Guid.NewGuid();
        public string Name { get; set; }
        public TaskItemType Type { get; set; }
        public TaskItemStatus Status { get; set; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public TimeSpan ExecutionTime { get; set; }
        public Guid OwnerUserId { get; set; }
        public string ParametersJson { get; set; }

        [NotMapped]
        public ITaskItemParameter? Parameters => DeserializeParameters();

        private ITaskItemParameter? DeserializeParameters()
        {
            if (string.IsNullOrWhiteSpace(ParametersJson))
                return null;

            return JsonConvert.DeserializeObject<ITaskItemParameter>(ParametersJson, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        }

        public TaskItem(string name, TaskItemType type, ITaskItemParameter parameters, Guid ownerUserId)
        {
            Name = name;
            Type = type;
            ParametersJson = parameters.ToJson();
            OwnerUserId = ownerUserId;
            CreatedAt = DateTime.UtcNow;
            Status = TaskItemStatus.Pending;
        }

    }
}
