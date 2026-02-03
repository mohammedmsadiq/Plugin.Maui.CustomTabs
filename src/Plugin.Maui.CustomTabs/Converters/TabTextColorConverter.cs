using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Plugin.Maui.CustomTabs.Models;

namespace Plugin.Maui.CustomTabs.Converters;

/// <summary>
/// Chooses a text color based on selection state.
/// </summary>
public sealed class TabTextColorConverter : IMultiValueConverter
{
    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 3)
        {
            return Colors.White;
        }

        var tab = values[0] as CustomTabItem;
        var selected = values[1] as CustomTabItem;
        var options = values[2] as CustomTabsOptions;
        var selectedOverride = values.Length > 3 ? ExtractColor(values[3]) : null;
        var unselectedOverride = values.Length > 4 ? ExtractColor(values[4]) : null;

        if (tab == null || options == null)
        {
            return Colors.White;
        }

        var selectedColor = selectedOverride ?? tab.SelectedTextColor ?? options.SelectedTextColor;
        var unselectedColor = unselectedOverride ?? tab.UnselectedTextColor ?? options.UnselectedTextColor;

        return ReferenceEquals(tab, selected) ? selectedColor : unselectedColor;
    }

    private static Color? ExtractColor(object? value)
    {
        return value is Color color ? color : null;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
