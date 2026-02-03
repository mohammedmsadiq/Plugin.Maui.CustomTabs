using System.Globalization;
using Microsoft.Maui.Controls;

namespace Plugin.Maui.CustomTabs.Converters;

/// <summary>
/// Returns the first non-empty string from a multi-binding.
/// </summary>
public sealed class FallbackTextConverter : IMultiValueConverter
{
    /// <inheritdoc />
    public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null)
        {
            return string.Empty;
        }

        foreach (var value in values)
        {
            if (value is string text && !string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
        }

        return string.Empty;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
