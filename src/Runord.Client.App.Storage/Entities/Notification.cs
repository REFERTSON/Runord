using Runord.Client.App.Storage.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runord.Client.App.Storage.Entities
{
    public class Notification : Entity
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public Guid OwnerUserId { get; init; }
        public DateTime Created { get; init; }

        public Notification(string title, string description, Guid ownerUserId, DateTime created)
        {
            Title = title;
            Description = description;
            OwnerUserId = ownerUserId;
            Created = created;  
        }
    }

}