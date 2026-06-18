using Runord.Client.Views.Windows;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Runord.Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) => ShowMain();

        // Общая настройка окна для режимов Login/Main (с видимым топбаром и изменяемыми размерами)
        private void SetNormalWindowLayout()
        {
            ResizeMode = ResizeMode.CanResize;
        }

        public void ShowLoading()
        {
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            MainContentHolder.Margin = new Thickness(0);
            SwitchView(new LoadingView());
        }

        public void ShowLogin()
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            SetNormalWindowLayout();
            SwitchView(new LoginView());
        }

        public void ShowMain()
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            SetNormalWindowLayout();
            SwitchView(new MainView());
        }

        // Плавная смена контента с анимацией прозрачности и сдвига
        private void SwitchView(FrameworkElement newView)
        {
            // Установка размеров окна под новое представление
            Width = newView.Width;
            Height = newView.Height;

            // Центрирование по горизонтали и смещение вверх на 2.5 от вертикального центра
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2.5;

            // Начальное состояние для анимации
            newView.Opacity = 0;
            var transform = new TranslateTransform(0, 10);
            newView.RenderTransform = transform;

            MainContentHolder.Content = newView;

            // Анимация появления
            var fadeAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(400))
            {
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
            };
            var slideAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(400))
            {
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
            };

            newView.BeginAnimation(OpacityProperty, fadeAnimation);
            transform.BeginAnimation(TranslateTransform.YProperty, slideAnimation);
        }
    }
}