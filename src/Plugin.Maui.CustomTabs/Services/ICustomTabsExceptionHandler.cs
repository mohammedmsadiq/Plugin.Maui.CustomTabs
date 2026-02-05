namespace Plugin.Maui.CustomTabs.Services;

/// <summary>
/// Handles exceptions raised from custom tabs internals.
/// </summary>
public interface ICustomTabsExceptionHandler
{
    /// <summary>
    /// Handles an exception with a context label.
    /// </summary>
    void Handle(Exception exception, string context);
}
