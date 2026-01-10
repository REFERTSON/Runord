using Runord.Client.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Runord.Client.App.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && DataContext is SettingsViewModel vm)
            {
                switch (btn.Tag)
                {
                    case "GeneralSettings": SettingsItemsControl.ItemsSource = vm.GeneralSettings; break;
                    case "NotificationSettings": SettingsItemsControl.ItemsSource = vm.NotificationSettings; break;
                    case "InterfaceSettings": SettingsItemsControl.ItemsSource = vm.InterfaceSettings; break;
                    case "NetworkSettings": SettingsItemsControl.ItemsSource = vm.NetworkSettings; break;
                    case "NodeSettings": SettingsItemsControl.ItemsSource = vm.NodeSettings; break;
                    case "SecuritySettings": SettingsItemsControl.ItemsSource = vm.SecuritySettings; break;
                }
            }
        }
    }
}
