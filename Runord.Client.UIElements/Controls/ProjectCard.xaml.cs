using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Runord.Client.UIElements.Controls
{
    public partial class ProjectCard : UserControl
    {
        public ProjectCard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ProjectNameProperty =
            DependencyProperty.Register("ProjectName", typeof(string), typeof(ProjectCard), new PropertyMetadata("Название проекта"));
        public string ProjectName
        {
            get => (string)GetValue(ProjectNameProperty);
            set => SetValue(ProjectNameProperty, value);
        }

        public static readonly DependencyProperty ProjectDescriptionProperty =
            DependencyProperty.Register("ProjectDescription", typeof(string), typeof(ProjectCard), new PropertyMetadata("Описание проекта отсутствует."));
        public string ProjectDescription
        {
            get => (string)GetValue(ProjectDescriptionProperty);
            set => SetValue(ProjectDescriptionProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(ProjectCard), new PropertyMetadata(null));
        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public static readonly DependencyProperty EditCommandParameterProperty =
            DependencyProperty.Register("EditCommandParameter", typeof(object), typeof(ProjectCard), new PropertyMetadata(null));
        public object EditCommandParameter
        {
            get => GetValue(EditCommandParameterProperty);
            set => SetValue(EditCommandParameterProperty, value);
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(ProjectCard), new PropertyMetadata(null));
        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public static readonly DependencyProperty DeleteCommandParameterProperty =
            DependencyProperty.Register("DeleteCommandParameter", typeof(object), typeof(ProjectCard), new PropertyMetadata(null));
        public object DeleteCommandParameter
        {
            get => GetValue(DeleteCommandParameterProperty);
            set => SetValue(DeleteCommandParameterProperty, value);
        }
    }
}