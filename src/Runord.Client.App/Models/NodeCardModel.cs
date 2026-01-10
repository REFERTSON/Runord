using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Runord.Client.App.Models
{
    public class NodeCardModel
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public bool IsOnline { get; set; }

        public string StatusText => IsOnline ? "В сети" : "Не в сети";
        public Brush StatusColor => IsOnline ? Brushes.LimeGreen : Brushes.IndianRed;

        public string Cpu => IsOnline ? "CPU: 18%" : "CPU: -";
        public string Ram => IsOnline ? "RAM: 18%" : "RAM: -";
        public string Tasks => IsOnline ? "TASKS: 152" : "TASKS: 0";
    }
}
