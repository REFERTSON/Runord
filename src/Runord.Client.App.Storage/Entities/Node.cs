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
    public class Node : Entity
    {
        public string Name { get; set; }
        public NodeAddress Address { get; set; }
        public NodeMonitoring Monitoring { get; set; }
        public NodeStatus Status { get; set; }

        public Node(string name, NodeAddress address) 
        {
            Name = name;
            Address = address;
            Monitoring = new NodeMonitoring(0, 0, 0);
            Status = NodeStatus.Unknow;
        }
    }
}
