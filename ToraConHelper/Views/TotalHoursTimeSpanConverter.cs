using System;
using System.Globalization;
using System.Windows.Data;

namespace ToraConHelper.Views;

internal class TotalHoursTimeSpanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var ts = value as TimeSpan?;
        if (ts == null) return string.Empty;

        return $"{Math.Floor(ts.Value.TotalHours)}:{ts.Value.Minutes.ToString("00")}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
