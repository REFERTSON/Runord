using Runord.Client.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Runord.Client.App.Converters
{
    public class TaskStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskItemStatus status)
                return TaskItemStatusDescriptions.GetDescription(status);
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class TaskStatusBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskItemStatus status)
            {
                return status switch
                {
                    TaskItemStatus.Pending => (Brush)new BrushConverter().ConvertFrom("#B0FFD859"),
                    TaskItemStatus.Running => (Brush)new BrushConverter().ConvertFrom("#B059AFFF"),
                    TaskItemStatus.Completed => (Brush)new BrushConverter().ConvertFrom("#B059FF91"),
                    TaskItemStatus.Failed => (Brush)new BrushConverter().ConvertFrom("#B0E94545"),
                    TaskItemStatus.Cancelled => (Brush)new BrushConverter().ConvertFrom("#B0999999"),
                    _ => Brushes.White
                };
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class TaskStatusBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskItemStatus status)
            {
                return status switch
                {
                    TaskItemStatus.Pending => (Brush)new BrushConverter().ConvertFrom("#D9FFD859"),
                    TaskItemStatus.Running => (Brush)new BrushConverter().ConvertFrom("#D959AFFF"),
                    TaskItemStatus.Completed => (Brush)new BrushConverter().ConvertFrom("#D959FF91"),
                    TaskItemStatus.Failed => (Brush)new BrushConverter().ConvertFrom("#D9E94545"),
                    TaskItemStatus.Cancelled => (Brush)new BrushConverter().ConvertFrom("#D9C0C0C0"),
                    _ => Brushes.Transparent
                };
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
