using System;
using System.Globalization;
using System.Windows.Data;

namespace ToraConHelper.Views;

public enum RemainingTimeWarning
{
    None,
    Yellow,
    Red
}

public class RemainingTimeWarningConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TimeSpan remaining) return RemainingTimeWarning.None;
        if (remaining <= TimeSpan.FromHours(1)) return RemainingTimeWarning.Red;
        if (remaining <= TimeSpan.FromHours(2)) return RemainingTimeWarning.Yellow;
        return RemainingTimeWarning.None;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
