using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Client.UIElements.Models.Sidebar
{
    public partial class SidebarUserProfile : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _role = string.Empty;

        [ObservableProperty]
        private string _icon_path = string.Empty;
    }
}
