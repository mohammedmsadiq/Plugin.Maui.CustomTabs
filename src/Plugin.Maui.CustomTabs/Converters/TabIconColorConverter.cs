using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Plugin.Maui.CustomTabs.Models;

namespace Plugin.Maui.CustomTabs.Converters;

/// <summary>
/// Chooses an icon color based on selection state.
/// </summary>
public sealed class TabIconColorConverter : IMultiValueConverter
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

        if (tab == null || options == null)
        {
            return Colors.White;
        }

        return ReferenceEquals(tab, selected) ? options.SelectedIconColor : options.UnselectedIconColor;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
