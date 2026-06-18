using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Runord.Client.UIElements.Controls
{
    public partial class NodeCard : UserControl
    {
        public NodeCard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty NodeNameProperty =
            DependencyProperty.Register("NodeName", typeof(string), typeof(NodeCard), new PropertyMetadata("NODE - 01"));
        public string NodeName
        {
            get => (string)GetValue(NodeNameProperty);
            set => SetValue(NodeNameProperty, value);
        }

        public static readonly DependencyProperty NodeIpProperty =
            DependencyProperty.Register("NodeIp", typeof(string), typeof(NodeCard), new PropertyMetadata("192.168.0.1"));
        public string NodeIp
        {
            get => (string)GetValue(NodeIpProperty);
            set => SetValue(NodeIpProperty, value);
        }

        public static readonly DependencyProperty NodeStatusTextProperty =
            DependencyProperty.Register("NodeStatusText", typeof(string), typeof(NodeCard), new PropertyMetadata("Не сети"));
        public string NodeStatusText
        {
            get => (string)GetValue(NodeStatusTextProperty);
            set => SetValue(NodeStatusTextProperty, value);
        }

        public static readonly DependencyProperty CpuUsageProperty =
            DependencyProperty.Register("CpuUsage", typeof(string), typeof(NodeCard), new PropertyMetadata("- % / 500 %"));
        public string CpuUsage
        {
            get => (string)GetValue(CpuUsageProperty);
            set => SetValue(CpuUsageProperty, value);
        }

        public static readonly DependencyProperty RamUsageProperty =
            DependencyProperty.Register("RamUsage", typeof(string), typeof(NodeCard), new PropertyMetadata("- Гб / 8 Гб"));
        public string RamUsage
        {
            get => (string)GetValue(RamUsageProperty);
            set => SetValue(RamUsageProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(NodeCard), new PropertyMetadata(null));
        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(NodeCard), new PropertyMetadata(null));
        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public static readonly DependencyProperty IsOnlineProperty =
            DependencyProperty.Register("IsOnline", typeof(bool), typeof(NodeCard), new PropertyMetadata(false));
        public bool IsOnline
        {
            get => (bool)GetValue(IsOnlineProperty);
            set => SetValue(IsOnlineProperty, value);
        }

        // ================= ДОБАВИТЬ ЭТОТ БЛОК СВОЙСТВ =================

        public static readonly DependencyProperty EditCommandParameterProperty =
            DependencyProperty.Register("EditCommandParameter", typeof(object), typeof(NodeCard), new PropertyMetadata(null));
        public object EditCommandParameter
        {
            get => GetValue(EditCommandParameterProperty);
            set => SetValue(EditCommandParameterProperty, value);
        }

        public static readonly DependencyProperty DeleteCommandParameterProperty =
            DependencyProperty.Register("DeleteCommandParameter", typeof(object), typeof(NodeCard), new PropertyMetadata(null));
        public object DeleteCommandParameter
        {
            get => GetValue(DeleteCommandParameterProperty);
            set => SetValue(DeleteCommandParameterProperty, value);
        }
    }
}