namespace Plugin.Maui.CustomTabs.Services;

/// <summary>
/// Optional localization service used to refresh tab titles when language changes.
/// </summary>
public interface ILocalizationRefreshService
{
    /// <summary>
    /// Raised when the application language changes.
    /// </summary>
    event EventHandler? LanguageChanged;
}
