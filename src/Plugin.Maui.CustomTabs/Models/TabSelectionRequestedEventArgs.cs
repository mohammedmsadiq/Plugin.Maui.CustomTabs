namespace Plugin.Maui.CustomTabs.Models;

/// <summary>
/// Provides data for tab selection requests.
/// </summary>
public sealed class TabSelectionRequestedEventArgs : EventArgs
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TabSelectionRequestedEventArgs(CustomTabItem tab, CustomTabItem? currentTab, TabSelectionOrigin origin, bool isReselect)
    {
        Tab = tab;
        CurrentTab = currentTab;
        Origin = origin;
        IsReselect = isReselect;
    }

    /// <summary>
    /// Tab being selected.
    /// </summary>
    public CustomTabItem Tab { get; }

    /// <summary>
    /// Currently selected tab (if any).
    /// </summary>
    public CustomTabItem? CurrentTab { get; }

    /// <summary>
    /// Selection origin.
    /// </summary>
    public TabSelectionOrigin Origin { get; }

    /// <summary>
    /// True when the user tapped the already-selected tab.
    /// </summary>
    public bool IsReselect { get; }

    /// <summary>
    /// Set to true to cancel the selection.
    /// </summary>
    public bool Cancel { get; set; }
}
