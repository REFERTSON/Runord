using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Runord.Client.App.ViewModels;

namespace Runord.Client.App.Views
{
    public partial class ConsoleView : UserControl
    {
        public ConsoleView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            DataContext = new ConsoleViewModel();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            CommandInput.Focus();
        }

        private void CommandInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is ConsoleViewModel viewModel)
                {
                    viewModel.ExecuteCommand.Execute(null);
                    ScrollToBottom();
                }
                e.Handled = true;
            }
        }

        private void ScrollToBottom()
        {
            if (ConsoleOutputItems.Items.Count > 0)
            {
                var lastItem = ConsoleOutputItems.Items[ConsoleOutputItems.Items.Count - 1];
                ConsoleOutputItems.ScrollIntoView(lastItem);
            }
        }
    }
}