using System;
using System.Globalization;
using System.Windows.Data;

namespace ToraConHelper.Views;

public class MphConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int kph = (int)value;
        return Math.Round(kph / 1.60934, 1);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
     => throw new NotImplementedException();
}
