namespace Plugin.Maui.CustomTabs.Models;

/// <summary>
/// Indicates what initiated a tab selection.
/// </summary>
public enum TabSelectionOrigin
{
    /// <summary>
    /// Selection initiated by user interaction.
    /// </summary>
    User,

    /// <summary>
    /// Selection initiated programmatically.
    /// </summary>
    Programmatic
}
