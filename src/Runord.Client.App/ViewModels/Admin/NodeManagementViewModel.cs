using Runord.Client.App.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runord.Client.App.ViewModels
{
    using Runord.Client.App.Models;
    using System.Collections.ObjectModel;

    public class NodeManagementViewModel : ViewModelBase
    {
        public ObservableCollection<NodeCardModel> Nodes { get; }

        public NodeManagementViewModel()
        {
            Nodes = new ObservableCollection<NodeCardModel>
        {
            new NodeCardModel
            {
                Name = "Узел #1453ghrhtrjtej",
                Ip = "192.168.1",
                IsOnline = false
            },
            new NodeCardModel
            {
                Name = "Узел #1",
                Ip = "192.168.1",
                IsOnline = true
            },
            new NodeCardModel
            {
                Name = "Узел #1",
                Ip = "192.168.1",
                IsOnline = false
            },
            new NodeCardModel
            {
                Name = "Узел #1",
                Ip = "192.168.1",
                IsOnline = true
            },
            new NodeCardModel
            {
                Name = "Узел #1",
                Ip = "192.168.1",
                IsOnline = false
            },
            new NodeCardModel
            {
                Name = "Узел #1",
                Ip = "192.168.1",
                IsOnline = true
            }
        };
        }
    }

}
