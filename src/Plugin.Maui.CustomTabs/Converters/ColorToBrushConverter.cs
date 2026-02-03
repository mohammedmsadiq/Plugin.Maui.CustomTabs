using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Plugin.Maui.CustomTabs.Converters;

/// <summary>
/// Converts a Color to a Brush for stroke bindings.
/// </summary>
public sealed class ColorToBrushConverter : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Brush brush)
        {
            return brush;
        }

        if (value is Color color)
        {
            return new SolidColorBrush(color);
        }

        return new SolidColorBrush(Colors.Transparent);
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
