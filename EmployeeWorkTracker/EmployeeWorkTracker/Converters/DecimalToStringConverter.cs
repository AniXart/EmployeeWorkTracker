using System.Globalization;
using System.Windows.Data;

namespace EmployeeWorkTracker.Converters;

public sealed class DecimalToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal d)
            return d.ToString("0.##", culture);
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s && decimal.TryParse(s, NumberStyles.Any, culture, out var result))
            return result;
        return 0m;
    }
}