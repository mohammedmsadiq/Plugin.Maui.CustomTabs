namespace Plugin.Maui.CustomTabs.Models;

/// <summary>
/// Defines how tabs are laid out in the tab bar.
/// </summary>
public enum TabLayoutMode
{
    /// <summary>
    /// Tabs take available space and do not scroll.
    /// </summary>
    Fixed,

    /// <summary>
    /// Tabs are horizontally scrollable.
    /// </summary>
    Scrollable,

    /// <summary>
    /// Chooses Fixed or Scrollable based on tab count.
    /// </summary>
    Auto
}
