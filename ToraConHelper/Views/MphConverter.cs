using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ToraConHelper.Views;

public class MphConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int kph = (int)value;
        return Math.Round(kph / 1.609344, 1);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
     => throw new NotImplementedException();
}


public class PowerToysNavigationDistanceConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2 || values[0] == null || values[1] == null) return string.Empty;

        float kph = (float)values[0];
        string gameName = (string)values[1];

        if (string.IsNullOrEmpty(gameName)) return string.Empty;
        StringBuilder formatted = new($"{kph:0} km");
        if (string.CompareOrdinal("ATS", gameName) == 0)
        {
            formatted.Append($" ({Math.Round(kph / 1.609344, 1):0} mile)");
        }
        return formatted.ToString();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
