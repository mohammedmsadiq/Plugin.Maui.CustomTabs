namespace Plugin.Maui.CustomTabs.Models;

/// <summary>
/// Provides data for tab selection changes.
/// </summary>
public sealed class TabSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TabSelectionChangedEventArgs(CustomTabItem? previousTab, CustomTabItem? currentTab, int selectedIndex)
    {
        PreviousTab = previousTab;
        CurrentTab = currentTab;
        SelectedIndex = selectedIndex;
        SelectedKey = currentTab?.Key;
    }

    /// <summary>
    /// Previously selected tab.
    /// </summary>
    public CustomTabItem? PreviousTab { get; }

    /// <summary>
    /// Currently selected tab.
    /// </summary>
    public CustomTabItem? CurrentTab { get; }

    /// <summary>
    /// Index of the selected tab within the tabs collection.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    /// Key of the selected tab, if any.
    /// </summary>
    public string? SelectedKey { get; }
}
