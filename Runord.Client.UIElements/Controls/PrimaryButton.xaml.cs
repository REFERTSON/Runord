using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;

namespace Runord.Client.UIElements.Controls
{
    /// <summary>
    /// Логика взаимодействия для PrimaryButton.xaml
    /// </summary>
    public partial class PrimaryButton : UserControl
    {
        public PrimaryButton()
        {
            InitializeComponent();
        }

        #region Dependency Properties (все с префиксом Button)

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(PrimaryButton), new PropertyMetadata(string.Empty));
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonIconUriProperty =
            DependencyProperty.Register("ButtonIconUri", typeof(string), typeof(PrimaryButton), new PropertyMetadata(null));
        public string ButtonIconUri
        {
            get => (string)GetValue(ButtonIconUriProperty);
            set => SetValue(ButtonIconUriProperty, value);
        }

        public static readonly DependencyProperty ButtonIconDataProperty =
            DependencyProperty.Register("ButtonIconData", typeof(Geometry), typeof(PrimaryButton), new PropertyMetadata(null));
        public Geometry ButtonIconData
        {
            get => (Geometry)GetValue(ButtonIconDataProperty);
            set => SetValue(ButtonIconDataProperty, value);
        }

        public static readonly DependencyProperty ButtonIconWidthProperty =
            DependencyProperty.Register("ButtonIconWidth", typeof(double), typeof(PrimaryButton), new PropertyMetadata(20.0));
        public double ButtonIconWidth
        {
            get => (double)GetValue(ButtonIconWidthProperty);
            set => SetValue(ButtonIconWidthProperty, value);
        }

        public static readonly DependencyProperty ButtonIconHeightProperty =
            DependencyProperty.Register("ButtonIconHeight", typeof(double), typeof(PrimaryButton), new PropertyMetadata(20.0));
        public double ButtonIconHeight
        {
            get => (double)GetValue(ButtonIconHeightProperty);
            set => SetValue(ButtonIconHeightProperty, value);
        }

        public static readonly DependencyProperty ButtonForegroundProperty =
            DependencyProperty.Register("ButtonForeground", typeof(Brush), typeof(PrimaryButton), new PropertyMetadata(Brushes.White));
        public Brush ButtonForeground
        {
            get => (Brush)GetValue(ButtonForegroundProperty);
            set => SetValue(ButtonForegroundProperty, value);
        }

        public static readonly DependencyProperty ButtonFontFamilyProperty =
            DependencyProperty.Register("ButtonFontFamily", typeof(FontFamily), typeof(PrimaryButton), new PropertyMetadata(new FontFamily("Segoe UI")));
        public FontFamily ButtonFontFamily
        {
            get => (FontFamily)GetValue(ButtonFontFamilyProperty);
            set => SetValue(ButtonFontFamilyProperty, value);
        }

        public static readonly DependencyProperty ButtonFontSizeProperty =
            DependencyProperty.Register("ButtonFontSize", typeof(double), typeof(PrimaryButton), new PropertyMetadata(16.0));
        public double ButtonFontSize
        {
            get => (double)GetValue(ButtonFontSizeProperty);
            set => SetValue(ButtonFontSizeProperty, value);
        }

        public static readonly DependencyProperty ButtonFontWeightProperty =
            DependencyProperty.Register("ButtonFontWeight", typeof(FontWeight), typeof(PrimaryButton), new PropertyMetadata(FontWeights.SemiBold));
        public FontWeight ButtonFontWeight
        {
            get => (FontWeight)GetValue(ButtonFontWeightProperty);
            set => SetValue(ButtonFontWeightProperty, value);
        }

        public static readonly DependencyProperty ButtonCornerRadiusProperty =
            DependencyProperty.Register("ButtonCornerRadius", typeof(CornerRadius), typeof(PrimaryButton), new PropertyMetadata(new CornerRadius(6)));
        public CornerRadius ButtonCornerRadius
        {
            get => (CornerRadius)GetValue(ButtonCornerRadiusProperty);
            set => SetValue(ButtonCornerRadiusProperty, value);
        }

        public static readonly DependencyProperty ButtonBackgroundProperty =
            DependencyProperty.Register("ButtonBackground", typeof(Brush), typeof(PrimaryButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(44, 44, 44))));
        public Brush ButtonBackground
        {
            get => (Brush)GetValue(ButtonBackgroundProperty);
            set => SetValue(ButtonBackgroundProperty, value);
        }

        public static readonly DependencyProperty ButtonHoverBackgroundProperty =
            DependencyProperty.Register("ButtonHoverBackground", typeof(Brush), typeof(PrimaryButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(64, 64, 64))));
        public Brush ButtonHoverBackground
        {
            get => (Brush)GetValue(ButtonHoverBackgroundProperty);
            set => SetValue(ButtonHoverBackgroundProperty, value);
        }

        public static readonly DependencyProperty ButtonPressedBackgroundProperty =
            DependencyProperty.Register("ButtonPressedBackground", typeof(Brush), typeof(PrimaryButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(26, 26, 26))));
        public Brush ButtonPressedBackground
        {
            get => (Brush)GetValue(ButtonPressedBackgroundProperty);
            set => SetValue(ButtonPressedBackgroundProperty, value);
        }

        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(PrimaryButton), new PropertyMetadata(null));
        public ICommand ButtonCommand
        {
            get => (ICommand)GetValue(ButtonCommandProperty);
            set => SetValue(ButtonCommandProperty, value);
        }

        public static readonly DependencyProperty ButtonCommandParameterProperty =
            DependencyProperty.Register("ButtonCommandParameter", typeof(object), typeof(PrimaryButton), new PropertyMetadata(null));
        public object ButtonCommandParameter
        {
            get => (object)GetValue(ButtonCommandParameterProperty);
            set => SetValue(ButtonCommandParameterProperty, value);
        }

        #endregion

        #region Click Event

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PrimaryButton));
        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent, this));
        }

        #endregion
    }
}