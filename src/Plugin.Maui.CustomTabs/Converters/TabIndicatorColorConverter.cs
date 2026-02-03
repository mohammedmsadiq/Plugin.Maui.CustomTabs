using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Plugin.Maui.CustomTabs.Models;

namespace Plugin.Maui.CustomTabs.Converters;

/// <summary>
/// Chooses an indicator color based on selection state.
/// </summary>
public sealed class TabIndicatorColorConverter : IMultiValueConverter
{
    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 3)
        {
            return Colors.Transparent;
        }

        var tab = values[0] as CustomTabItem;
        var selected = values[1] as CustomTabItem;
        var options = values[2] as CustomTabsOptions;
        var indicatorOverride = values.Length > 3 ? ExtractColor(values[3]) : null;

        if (tab == null || options == null)
        {
            return Colors.Transparent;
        }

        if (!ReferenceEquals(tab, selected))
        {
            return Colors.Transparent;
        }

        return indicatorOverride ?? tab.IndicatorColor ?? options.AccentColor;
    }

    private static Color? ExtractColor(object? value)
    {
        return value is Color color ? color : null;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
