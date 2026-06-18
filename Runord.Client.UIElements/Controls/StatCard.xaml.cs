using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;  // Для Brush

namespace Runord.Client.UIElements.Controls
{
    /// <summary>
    /// Логика взаимодействия для StatCard.xaml
    /// </summary>
    public partial class StatCard : UserControl
    {
        public StatCard()
        {
            InitializeComponent();
        }

        // Свойство для Заголовка ("КОЛ-ВО ЗАДАЧ", "ОШИБКА")
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(StatCard));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Свойство для Числа (2566, 200)
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(StatCard));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Свойство для Цвета полосы (Зеленый, Красный, Серый)
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(StatCard));

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
    }
}
