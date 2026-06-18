using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Runord.Client.UIElements.Controls
{
    public partial class SidebarMenuButton : UserControl
    {
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof(Uri), typeof(SidebarMenuButton));

        public Uri IconSource
        {
            get { return (Uri)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        public static readonly DependencyProperty TextButtonProperty =
            DependencyProperty.Register("TextButton", typeof(string), typeof(SidebarMenuButton));

        public string TextButton
        {
            get { return (string)GetValue(TextButtonProperty); }
            set { SetValue(TextButtonProperty, value); }
        }

        public static readonly DependencyProperty TextButtonColorProperty =
            DependencyProperty.Register("TextButtonColor", typeof(Brush), typeof(SidebarMenuButton),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6C727F"))));

        public Brush TextButtonColor
        {
            get { return (Brush)GetValue(TextButtonColorProperty); }
            set { SetValue(TextButtonColorProperty, value); }
        }

        public static readonly DependencyProperty StripeColorProperty =
            DependencyProperty.Register("StripeColor", typeof(Brush), typeof(SidebarMenuButton),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F25555"))));

        public Brush StripeColor
        {
            get { return (Brush)GetValue(StripeColorProperty); }
            set { SetValue(StripeColorProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(SidebarMenuButton),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        // СВОЙСТВО СЧЕТЧИКА: Хранит строку текста для отображения на бейдже
        public static readonly DependencyProperty NotificationCountProperty =
            DependencyProperty.Register("NotificationCount", typeof(string), typeof(SidebarMenuButton), new PropertyMetadata(null));

        public string NotificationCount
        {
            get { return (string)GetValue(NotificationCountProperty); }
            set { SetValue(NotificationCountProperty, value); }
        }

        public SidebarMenuButton()
        {
            InitializeComponent();
        }
    }
}