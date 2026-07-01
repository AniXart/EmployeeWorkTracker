using System.Globalization;
using System.Windows.Data;

namespace EmployeeWorkTracker.Converters;

public sealed class EnumToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null || parameter is null)
            return false;

        return value.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && boolValue)
        {
            return parameter;
        }
        return Binding.DoNothing;
    }
}