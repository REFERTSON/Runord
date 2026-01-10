using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Runord.Client.App.Converters
{
    public class SortDirectionToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SortDirection dir)
            {
                return dir == SortDirection.Ascending
                    ? new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Icons/Logo.png"))
                    : new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Icons/Logo.png"));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
