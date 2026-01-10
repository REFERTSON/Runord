using Runord.Client.App.Storage.Entities.Base;
using Runord.Client.Shared.Enums;
using Runord.Client.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runord.Client.App.Storage.Entities
{
    public class HubNode : Entity
    {
        public string Name { get; set; }
        public NodeAddress Address { get; set; }
        public NodeStatus Status { get; set; }

        public HubNode(string name, NodeAddress address)
        {
            Name = name;
            Address = address;
            Status = NodeStatus.Unknow;
        }
    }
}
