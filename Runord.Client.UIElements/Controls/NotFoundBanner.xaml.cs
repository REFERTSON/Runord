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

namespace Runord.Client.UIElements.Controls
{
    /// <summary>
    /// Логика взаимодействия для NotFoundBanner.xaml
    /// </summary>
    public partial class NotFoundBanner : UserControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(NotFoundBanner));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Свойство для Заголовка ("КОЛ-ВО ЗАДАЧ", "ОШИБКА")
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(NotFoundBanner));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public NotFoundBanner()
        {
            InitializeComponent();
        }
    }
}
