namespace Plugin.Maui.CustomTabs.Models;

/// <summary>
/// Preset visual styles for the tab UI.
/// </summary>
public enum TabVisualStyle
{
    /// <summary>
    /// Option 1: classic bottom tabs.
    /// </summary>
    ClassicBottom = 1,

    /// <summary>
    /// Option 2: top tabs with underline.
    /// </summary>
    TopUnderline = 2,

    /// <summary>
    /// Option 3: segmented control style.
    /// </summary>
    Segmented = 3,

    /// <summary>
    /// Option 4: floating rounded tab bar.
    /// </summary>
    Floating = 4,

    /// <summary>
    /// Option 5: reserved for navigation rail style.
    /// </summary>
    NavigationRail = 5,

    /// <summary>
    /// Option 6: icon-only tabs.
    /// </summary>
    IconOnly = 6,

    /// <summary>
    /// Option 7: reserved for center-action tab style.
    /// </summary>
    CenterAction = 7,

    /// <summary>
    /// Option 8: compact tabs.
    /// </summary>
    Compact = 8,

    /// <summary>
    /// Option 9: minimal text-first tabs.
    /// </summary>
    Minimal = 9,

    /// <summary>
    /// Option 10: pill tabs.
    /// </summary>
    Pills = 10
}
