using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EmployeeWorkTracker.Converters;

public sealed class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isNull = value is null;
        bool isEmpty = value is string s && string.IsNullOrEmpty(s);

        if (parameter is string p && p == "inverted")
        {
            // Инвертированный: показать если null или пусто
            return (isNull || isEmpty) ? Visibility.Visible : Visibility.Collapsed;
        }

        if (parameter is string p2 && p2 == "nonempty")
        {
            // Показать если НЕ null и НЕ пусто
            return (isNull || isEmpty) ? Visibility.Collapsed : Visibility.Visible;
        }

        // Обычный: показать если НЕ null
        return isNull ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}