using Microsoft.Maui.Controls;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Pages;
using Plugin.Maui.CustomTabs.Services;
using Plugin.Maui.CustomTabs.ViewModels;

namespace Plugin.Maui.CustomTabs.Extensions;

/// <summary>
/// Factory for creating custom tab host pages.
/// </summary>
public static class CustomTabs
{
    /// <summary>
    /// Creates a custom tabs host page.
    /// </summary>
    /// <param name="tabs">Tabs to display.</param>
    /// <param name="defaultKey">Key of the default selected tab.</param>
    /// <param name="options">Optional tab bar options.</param>
    /// <param name="localizationService">Optional localization refresh service.</param>
    public static Page Create(
        IEnumerable<CustomTabItem> tabs,
        string defaultKey,
        CustomTabsOptions? options = null,
        ILocalizationRefreshService? localizationService = null)
    {
        var resolvedOptions = options ?? new CustomTabsOptions();
        var viewModel = new CustomTabsViewModel(tabs, defaultKey, resolvedOptions);
        return new CustomTabsHostPage(viewModel, localizationService);
    }
}
