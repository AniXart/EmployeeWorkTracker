using System.Globalization;
using System.Windows.Data;

namespace EmployeeWorkTracker.Converters;

public sealed class IndexOfConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is System.Collections.IList list && parameter is string param)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is string item && item == param)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}