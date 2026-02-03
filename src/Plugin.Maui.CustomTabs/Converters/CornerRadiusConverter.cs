using System.Globalization;
using Microsoft.Maui.Controls;

namespace Plugin.Maui.CustomTabs.Converters;

/// <summary>
/// Converts numeric values to a CornerRadius.
/// </summary>
public sealed class CornerRadiusConverter : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            CornerRadius cornerRadius => cornerRadius,
            float floatValue => new CornerRadius(floatValue),
            double doubleValue => new CornerRadius(doubleValue),
            int intValue => new CornerRadius(intValue),
            _ => new CornerRadius(0)
        };
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
