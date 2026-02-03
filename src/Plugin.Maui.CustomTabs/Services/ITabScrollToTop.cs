namespace Plugin.Maui.CustomTabs.Services;

/// <summary>
/// Optional interface for pages that can scroll their content to the top.
/// </summary>
public interface ITabScrollToTop
{
    /// <summary>
    /// Scrolls the page content to the top.
    /// </summary>
    Task ScrollToTopAsync();
}
