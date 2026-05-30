using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Runord.Client.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для LoadingView.xaml
    /// </summary>
    public partial class LoadingView : UserControl
    {
        public LoadingView()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            int counter = 0;
            String status = "Подключение к хабу";

            for (int i = 0; i < 20; i++)
            {
                StatusText.Text = status + String.Concat(Enumerable.Repeat(".", counter < 4 ? counter++ : counter = 0));
                await Task.Delay(300);
            }

            for (int i = 0; i < 20; i++)
            {
                StatusText.Text = "Проверка работы сервисов" + String.Concat(Enumerable.Repeat(".", counter < 4 ? counter++ : counter = 0));
                await Task.Delay(300);
            }
        }
    }
}
