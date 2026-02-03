using System.Globalization;
using Microsoft.Maui.Controls;

namespace Plugin.Maui.CustomTabs.Converters;

/// <summary>
/// Compares two objects for equality in a multi-binding scenario.
/// </summary>
public sealed class EqualsConverter : IMultiValueConverter
{
    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 2)
        {
            return targetType == typeof(double) ? 0d : false;
        }

        var equals = Equals(values[0], values[1]);
        if (targetType == typeof(double))
        {
            return equals ? 1d : 0d;
        }

        return equals;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
