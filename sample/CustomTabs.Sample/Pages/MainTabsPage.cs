using CustomTabs.Sample.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Controls;
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
    public MainTabsPage()
        : this(ResolveLocalizationService())
    {
        Debug.WriteLine("[Sample] MainTabsPage constructor invoked.");
    }

    public MainTabsPage(SimpleLocalizationService localizationService)
        : base(CreateViewModel(localizationService), localizationService)
    {
        Title = "Custom Tabs";
        NavigationPage.SetHasNavigationBar(this, true);
        NavigationPage.SetTitleView(this, BuildTitleView());
        Debug.WriteLine("[Sample] MainTabsPage constructor invoked.");
    }

    private static View BuildTitleView()
        => new Grid
        {
            Padding = new Thickness(12, 0),
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                new Label
                {
                    Text = "Custom Tabs",
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.Black,
                    VerticalOptions = LayoutOptions.Center
                }
            }
        };

    protected override void OnAppearing()
    {
        try
        {
            base.OnAppearing();
            Debug.WriteLine("[Sample] MainTabsPage OnAppearing.");
        }
        catch (Exception ex)
        {
            SampleExceptionHandler.Report(ex, "MainTabsPage.OnAppearing");
        }
    }

    private static CustomTabsViewModel CreateViewModel(SimpleLocalizationService localizationService)
    {
        try
        {
            Debug.WriteLine("[Sample] MainTabsPage CreateViewModel starting.");
            var options = new CustomTabsOptions
            {
                // Keep host background aligned with sample theme (dark surface)
                // so extracted tab content remains readable.
                BackgroundColor = Colors.DarkBlue,
                PageBackgroundColor = Colors.Blue,
                ContentBackgroundColor = Colors.Green,
                AccentColor = Color.FromArgb("#E5E7EB"),
                SelectedIconColor = Color.FromArgb("#F9FAFB"),
                UnselectedIconColor = Color.FromArgb("#6B7280"),
                SelectedTextColor = Color.FromArgb("#F9FAFB"),
                UnselectedTextColor = Color.FromArgb("#9CA3AF"),
                ShowText = true,
                EnableAnimations = true,
                EnableHaptics = false,
                TabBarHeight = 52,
                TabBarPadding = new Thickness(8, 4, 8, 6),
                IconSize = 22,
                TextSize = 11,
                TabLayoutMode = TabLayoutMode.Fixed,
                ScrollableThreshold = int.MaxValue,
                BorderColor = Color.FromArgb("#374151"),
                BorderThickness = 1,
                UnderlineMargin = new Thickness(0, 6, 0, 0),
                ReselectBehavior = TabReselectBehavior.PopToRoot | TabReselectBehavior.ScrollToTop,
                VisualStyle = TabVisualStyle.ClassicBottom
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
                new CustomTabItem("settings", "G", () => new SettingsPage(options, localizationService))
                {
                    Title = "Settings",
                    TitleProvider = () => localizationService.Translate("Settings"),
                    AutomationId = "tab-settings",
                    AutomationName = "Settings Tab"
                }
            };

            return new CustomTabsViewModel(tabs, "home", options);
        }
        catch (Exception ex)
        {
            SampleExceptionHandler.Report(ex, "MainTabsPage.CreateViewModel");
            var fallbackTab = new CustomTabItem("error", "E", () => new ContentPage
            {
                Title = "Error",
                Content = new Label
                {
                    Margin = new Thickness(24),
                    Text = $"Main tabs failed to initialize.\n{ex.Message}"
                }
            })
            {
                Title = "Error"
            };

            return new CustomTabsViewModel(new[] { fallbackTab }, "error", new CustomTabsOptions());
        }
    }

    private static SimpleLocalizationService ResolveLocalizationService()
    {
        var services = Application.Current?.Handler?.MauiContext?.Services;
        if (services == null)
        {
            return new SimpleLocalizationService();
        }

        return services.GetService<SimpleLocalizationService>() ?? new SimpleLocalizationService();
    }
}
