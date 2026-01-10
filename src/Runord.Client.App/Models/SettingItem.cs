using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runord.Client.App.Models
{
    public class SettingItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public object Value { get; set; }
        public List<string>? Options { get; set; } // для ComboBox
        public SettingType Type { get; set; }
    }

    public enum SettingType
    {
        Boolean,
        String,
        Number,
        Option
    }

}
