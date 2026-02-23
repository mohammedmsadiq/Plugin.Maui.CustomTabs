using CustomTabs.Prism.Sample.Pages;
using Microsoft.Maui.Controls;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Pages;
using Plugin.Maui.CustomTabs.Prism;
using Plugin.Maui.CustomTabs.ViewModels;
using Prism.Ioc;

namespace CustomTabs.Prism.Sample.Pages;

/// <summary>
/// Prism-friendly root tabs page that hosts the custom tab bar.
/// </summary>
public sealed class MainTabsPage : CustomTabsHostPage
{
    private static bool _optionsConfigured;

    /// <summary>
    /// Creates the main tabs page.
    /// </summary>
    public MainTabsPage(IContainerProvider container, CustomTabsOptions options)
        : base(CreateViewModel(container, options), localizationService: null)
    {
        Title = "Prism Tabs";
        NavigationPage.SetHasNavigationBar(this, true);
        PrismCustomTabs.TrySetContainerProvider(this, container);
    }

    private static CustomTabsViewModel CreateViewModel(IContainerProvider container, CustomTabsOptions options)
    {
        ConfigureOptions(options);

        var tabs = new List<CustomTabItem>
        {
            PrismCustomTabs.CreateTab<HomePage>(container, "home", "H", tab =>
            {
                tab.Title = "Home";
                tab.AutomationId = "tab-home";
            }),
            PrismCustomTabs.CreateTab<SearchPage>(container, "search", "S", tab =>
            {
                tab.Title = "Search";
                tab.AutomationId = "tab-search";
            }),
            PrismCustomTabs.CreateTab<SettingsPage>(container, "settings", "G", tab =>
            {
                tab.Title = "Settings";
                tab.AutomationId = "tab-settings";
            })
        };

        return PrismCustomTabs.CreateViewModel(tabs, "home", options);
    }

    private static void ConfigureOptions(CustomTabsOptions options)
    {
        if (_optionsConfigured)
        {
            return;
        }

        _optionsConfigured = true;
        options.BackgroundColor = Color.FromArgb("#858c9d");
        options.AccentColor = Color.FromArgb("#E2E8F0");
        options.SelectedIconColor = Color.FromArgb("#F8FAFC");
        options.UnselectedIconColor = Color.FromArgb("#94A3B8");
        options.SelectedTextColor = Color.FromArgb("#F8FAFC");
        options.UnselectedTextColor = Color.FromArgb("#94A3B8");
        options.TabBarHeight = 56;
        options.TabBarPadding = new Thickness(0, 6, 0, 0);
        options.IconSize = 22;
        options.TextSize = 11;
        options.TabLayoutMode = TabLayoutMode.Fixed;
        options.BorderColor = Color.FromArgb("#334155");
        options.BorderThickness = 1;
        options.VisualStyle = TabVisualStyle.ClassicBottom;
    }
}
