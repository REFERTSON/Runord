using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Runord.Client.App.Controls
{
    public partial class CustomComboBox : UserControl
    {
        public CustomComboBox()
        {
            InitializeComponent();
            PART_Popup.Closed += (s, e) => IsDropDownOpen = false;
            this.PreviewMouseWheel += OnPreviewMouseWheel;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (oldParent is UIElement oldElement)
            {
                oldElement.PreviewMouseWheel -= OnPreviewMouseWheel;
            }
        }

        private void OnPreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (IsDropDownOpen)
            {
                IsDropDownOpen = false;
            }
        }

        // Свойства
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable),
                typeof(CustomComboBox), new PropertyMetadata(null, OnItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(object),
                typeof(CustomComboBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedValueChanged));

        public object SelectedValue
        {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(nameof(SelectedValuePath), typeof(string),
                typeof(CustomComboBox), new PropertyMetadata(null));

        public string SelectedValuePath
        {
            get => (string)GetValue(SelectedValuePathProperty);
            set => SetValue(SelectedValuePathProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string),
                typeof(CustomComboBox), new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool),
                typeof(CustomComboBox),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        // Обработчики
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomComboBox)d;
            control.RefreshItems();
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomComboBox)d;
            control.UpdateDisplayText();
        }

        private void RefreshItems()
        {
            PART_ItemsPanel.Children.Clear();

            if (ItemsSource == null) return;

            foreach (var item in ItemsSource)
            {
                var button = new Button
                {
                    Content = item.ToString(),
                    Background = (Brush)FindResource("CustomComboBoxItemBackgroundBrush"),
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(12, 8, 12, 8),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = (Brush)FindResource("CustomComboBoxItemForegroundBrush"),
                    Tag = item
                };

                button.Click += (s, e) =>
                {
                    var btn = (Button)s;
                    var selected = btn.Tag;

                    if (!string.IsNullOrEmpty(SelectedValuePath))
                    {
                        var prop = selected.GetType().GetProperty(SelectedValuePath);
                        if (prop != null)
                        {
                            SelectedValue = prop.GetValue(selected);
                        }
                    }
                    else
                    {
                        SelectedValue = selected;
                    }

                    IsDropDownOpen = false;
                };

                PART_ItemsPanel.Children.Add(button);
            }
        }

        private void UpdateDisplayText()
        {
            if (SelectedValue != null)
            {
                PART_DisplayText.Text = GetDisplayTextForValue(SelectedValue);
                PART_DisplayText.Foreground = (Brush)FindResource("CustomComboBoxForegroundBrush");
            }
            else
            {
                PART_DisplayText.Text = Placeholder;
                PART_DisplayText.Foreground = (Brush)FindResource("CustomComboBoxPlaceholderBrush");
            }
        }

        private string GetDisplayTextForValue(object value)
        {
            if (ItemsSource == null) return value?.ToString();

            foreach (var item in ItemsSource)
            {
                var val = item;
                if (!string.IsNullOrEmpty(SelectedValuePath))
                {
                    var prop = item.GetType().GetProperty(SelectedValuePath);
                    if (prop != null)
                    {
                        val = prop.GetValue(item);
                    }
                }

                if (Equals(val, value))
                {
                    return item.ToString();
                }
            }

            return value?.ToString();
        }

        private void PART_DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = !IsDropDownOpen;
        }

        private void PART_ArrowButton_Click(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = !IsDropDownOpen;
        }
    }
}