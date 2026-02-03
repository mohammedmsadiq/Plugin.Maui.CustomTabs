using CustomTabs.Sample.Services;
using Microsoft.Maui.Devices;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Pages;
using Plugin.Maui.CustomTabs.ViewModels;
using System.Diagnostics;

namespace CustomTabs.Sample.Pages;

/// <summary>
/// Prism-friendly root tabs page that hosts the custom tab bar.
/// </summary>
public sealed class MainTabsPage : CustomTabsHostPage
{
    /// <summary>
    /// Creates the main tabs page.
    /// </summary>
    public MainTabsPage(SimpleLocalizationService localizationService)
        : base(CreateViewModel(localizationService), localizationService)
    {
        Debug.WriteLine("[Sample] MainTabsPage constructor invoked.");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("[Sample] MainTabsPage OnAppearing.");
    }

    private static CustomTabsViewModel CreateViewModel(SimpleLocalizationService localizationService)
    {
        Debug.WriteLine("[Sample] MainTabsPage CreateViewModel starting.");
        var options = new CustomTabsOptions
        {
            BackgroundColor = Color.FromArgb("#071A3A"),
            AccentColor = Color.FromArgb("#D4AF37"),
            SelectedIconColor = Color.FromArgb("#D4AF37"),
            UnselectedIconColor = Colors.LightGray,
            SelectedTextColor = Colors.White,
            UnselectedTextColor = Colors.LightGray,
            ShowText = true,
            EnableAnimations = true,
            EnableHaptics = false,
            UnderlineMargin = new Thickness(0, 8, 0, 0),
            ReselectBehavior = TabReselectBehavior.PopToRoot | TabReselectBehavior.ScrollToTop
        };

        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            options.BadgeOffsetY = 0;
        }

        var messagesBadge = new CustomTabBadge
        {
            Count = 2,
            BackgroundColor = Colors.Red,
            TextColor = Colors.White
        };

        var tabs = new List<CustomTabItem>
        {
            new CustomTabItem("home", "H", () => new HomePage())
            {
                Title = "Home",
                TitleProvider = () => localizationService.Translate("Home"),
                AutomationId = "tab-home",
                AutomationName = "Home Tab"
            },
            new CustomTabItem("search", "S", () => new SearchPage())
            {
                Title = "Search",
                TitleProvider = () => localizationService.Translate("Search"),
                AutomationId = "tab-search",
                AutomationName = "Search Tab"
            },
            new CustomTabItem("messages", "M", () => new MessagesPage(messagesBadge))
            {
                Title = "Messages",
                TitleProvider = () => localizationService.Translate("Messages"),
                Badge = messagesBadge,
                AutomationId = "tab-messages",
                AutomationName = "Messages Tab"
            },
            new CustomTabItem("profile", "P", () => new ProfilePage())
            {
                Title = "Profile",
                TitleProvider = () => localizationService.Translate("Profile"),
                AutomationId = "tab-profile",
                AutomationName = "Profile Tab"
            },
            new CustomTabItem("settings", "T", () => new SettingsPage(options, localizationService))
            {
                Title = "Settings",
                TitleProvider = () => localizationService.Translate("Settings"),
                AutomationId = "tab-settings",
                AutomationName = "Settings Tab"
            }
        };

        return new CustomTabsViewModel(tabs, "home", options);
    }
}
