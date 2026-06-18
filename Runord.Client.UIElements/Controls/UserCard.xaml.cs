using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Runord.Client.UIElements.Controls
{
    public partial class UserCard : UserControl
    {
        public UserCard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(UserCard), new PropertyMetadata("Пользователь"));
        public string UserName { get => (string)GetValue(UserNameProperty); set => SetValue(UserNameProperty, value); }

        public static readonly DependencyProperty UserRoleProperty =
            DependencyProperty.Register("UserRole", typeof(string), typeof(UserCard), new PropertyMetadata("Роль"));
        public string UserRole { get => (string)GetValue(UserRoleProperty); set => SetValue(UserRoleProperty, value); }

        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof(string), typeof(UserCard), new PropertyMetadata("email@domain.com"));
        public string Email { get => (string)GetValue(EmailProperty); set => SetValue(EmailProperty, value); }

        public static readonly DependencyProperty LastActivityProperty =
            DependencyProperty.Register("LastActivity", typeof(string), typeof(UserCard), new PropertyMetadata("Неизвестно"));
        public string LastActivity { get => (string)GetValue(LastActivityProperty); set => SetValue(LastActivityProperty, value); }

        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register("StatusText", typeof(string), typeof(UserCard), new PropertyMetadata("Оффлайн"));
        public string StatusText { get => (string)GetValue(StatusTextProperty); set => SetValue(StatusTextProperty, value); }

        public static readonly DependencyProperty IsOnlineProperty =
            DependencyProperty.Register("IsOnline", typeof(bool), typeof(UserCard), new PropertyMetadata(false));
        public bool IsOnline { get => (bool)GetValue(IsOnlineProperty); set => SetValue(IsOnlineProperty, value); }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(UserCard));
        public ICommand EditCommand { get => (ICommand)GetValue(EditCommandProperty); set => SetValue(EditCommandProperty, value); }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(UserCard));
        public ICommand DeleteCommand { get => (ICommand)GetValue(DeleteCommandProperty); set => SetValue(DeleteCommandProperty, value); }

        public static readonly DependencyProperty EditCommandParameterProperty =
            DependencyProperty.Register("EditCommandParameter", typeof(object), typeof(UserCard));
        public object EditCommandParameter { get => GetValue(EditCommandParameterProperty); set => SetValue(EditCommandParameterProperty, value); }

        public static readonly DependencyProperty DeleteCommandParameterProperty =
            DependencyProperty.Register("DeleteCommandParameter", typeof(object), typeof(UserCard));
        public object DeleteCommandParameter { get => GetValue(DeleteCommandParameterProperty); set => SetValue(DeleteCommandParameterProperty, value); }
    }
}