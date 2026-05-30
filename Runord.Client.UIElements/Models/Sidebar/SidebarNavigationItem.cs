using CommunityToolkit.Mvvm.ComponentModel;


namespace Runord.Client.UIElements.Models
{
    public partial class SidebarNavigationItem : ObservableObject
    {
        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _iconData = string.Empty; // Будем хранить SVG-пути (Path Data)

        [ObservableProperty]
        private bool _isSelected;

        public Action? ExecuteAction { get; set; }
    }
}
