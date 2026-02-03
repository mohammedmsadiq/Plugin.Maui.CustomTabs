using System;

namespace Plugin.Maui.CustomTabs.Models;

/// <summary>
/// Defines behaviors to run when the active tab is tapped again.
/// </summary>
[Flags]
public enum TabReselectBehavior
{
    /// <summary>
    /// No reselect behavior.
    /// </summary>
    None = 0,

    /// <summary>
    /// Pops the current tab's navigation stack to root.
    /// </summary>
    PopToRoot = 1,

    /// <summary>
    /// Requests a scroll-to-top action on the current page.
    /// </summary>
    ScrollToTop = 2,

    /// <summary>
    /// Executes the tab reselect command.
    /// </summary>
    Command = 4
}
