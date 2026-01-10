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

namespace Runord.Client.App.Controls
{
    /// <summary>
    /// Логика взаимодействия для SearchTextBox.xaml
    /// </summary>
    public partial class SearchBox : UserControl
    {
        public SearchBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(
                nameof(SearchText),
                typeof(string),
                typeof(SearchBox),
                new PropertyMetadata(string.Empty));

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        private void SearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Находим окно, чтобы обработать клик по нему
            if (Window.GetWindow(this) is Window window)
            {
                window.PreviewMouseDown += Window_PreviewMouseDown;
            }
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Если клик не по TextBox — снимаем фокус
            if (!PART_SearchTextBox.IsMouseOver)
            {
                Keyboard.ClearFocus();
            }
        }
    }
}
