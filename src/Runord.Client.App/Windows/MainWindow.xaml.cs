using System.Windows;
using System.Windows.Controls;
using Runord.Client.App.ViewModels;

namespace Runord.Client.App.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}