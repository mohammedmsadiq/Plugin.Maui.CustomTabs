namespace Plugin.Maui.CustomTabs.Models;

/// <summary>
/// Result of a tab selection attempt.
/// </summary>
public enum TabSelectionResult
{
    /// <summary>
    /// No selection occurred.
    /// </summary>
    None,

    /// <summary>
    /// Tab selection was blocked by validation.
    /// </summary>
    Blocked,

    /// <summary>
    /// A new tab was selected.
    /// </summary>
    Selected,

    /// <summary>
    /// The currently selected tab was reselected.
    /// </summary>
    Reselected
}
