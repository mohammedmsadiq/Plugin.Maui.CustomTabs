using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Plugin.Maui.CustomTabs.Models;

namespace Plugin.Maui.CustomTabs.Converters;

/// <summary>
/// Chooses a tab item container background color based on selection state.
/// </summary>
public sealed class TabItemBackgroundColorConverter : IMultiValueConverter
{
    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 4)
        {
            return Colors.Transparent;
        }

        var tab = values[0] as CustomTabItem;
        var selected = values[1] as CustomTabItem;
        var selectedColor = values[2] as Color;
        var unselectedColor = values[3] as Color;

        if (tab == null)
        {
            return Colors.Transparent;
        }

        return ReferenceEquals(tab, selected)
            ? selectedColor ?? Colors.Transparent
            : unselectedColor ?? Colors.Transparent;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
