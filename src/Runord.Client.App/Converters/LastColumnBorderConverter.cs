using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Runord.Client.App.Converters
{
    public class LastColumnBorderConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int index && values[1] is int count)
                return index == count - 1
                    ? new Thickness(0, 0, 0, 2)   // последняя – без правой границы
                    : new Thickness(0, 0, 2, 2);  // обычная
            return new Thickness(0, 0, 2, 2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
