using System.Globalization;
using System.Windows.Data;

namespace EmployeeWorkTracker.Converters;

public sealed class IntToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int i)
            return i.ToString();
        return "0";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s && int.TryParse(s, out var result))
            return result;
        return 0;
    }
}