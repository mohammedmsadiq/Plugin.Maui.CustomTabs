using CustomTabs.Sample.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Pages;
using Plugin.Maui.CustomTabs.Prism;
using Plugin.Maui.CustomTabs.ViewModels;
using Prism.Ioc;
using System.Diagnostics;

namespace CustomTabs.Sample.Pages;

/// <summary>
/// Prism-friendly root tabs page that hosts the custom tab bar.
/// </summary>
public sealed class MainTabsPage : CustomTabsHostPage
{
    private static bool _optionsConfigured;
    private static bool _badgeConfigured;

    /// <summary>
    /// Creates the main tabs page.
    /// </summary>
    public MainTabsPage(IContainerProvider container,
        SimpleLocalizationService localizationService,
        CustomTabsOptions options,
        CustomTabBadge messagesBadge)
        : base(CreateViewModel(container, localizationService, options, messagesBadge), localizationService)
    {
        Title = "Custom Tabs";
        NavigationPage.SetHasNavigationBar(this, true);
        NavigationPage.SetTitleView(this, BuildTitleView());
        PrismCustomTabs.TrySetContainerProvider(this, container);
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

    private static CustomTabsViewModel CreateViewModel(
        IContainerProvider container,
        SimpleLocalizationService localizationService,
        CustomTabsOptions options,
        CustomTabBadge messagesBadge)
    {
        try
        {
            Debug.WriteLine("[Sample] MainTabsPage CreateViewModel starting.");
            ConfigureOptions(options);
            ConfigureBadge(messagesBadge);

            var tabs = new List<CustomTabItem>
            {
                PrismCustomTabs.CreateTab<HomePage>(container, "home", "H", tab =>
                {
                    tab.Title = "Home";
                    tab.TitleProvider = () => localizationService.Translate("Home");
                    tab.AutomationId = "tab-home";
                    tab.AutomationName = "Home Tab";
                }),
                PrismCustomTabs.CreateTab<SearchPage>(container, "search", "S", tab =>
                {
                    tab.Title = "Search";
                    tab.TitleProvider = () => localizationService.Translate("Search");
                    tab.AutomationId = "tab-search";
                    tab.AutomationName = "Search Tab";
                }),
                PrismCustomTabs.CreateTab<MessagesPage>(container, "messages", "M", tab =>
                {
                    tab.Title = "Messages";
                    tab.TitleProvider = () => localizationService.Translate("Messages");
                    tab.Badge = messagesBadge;
                    tab.AutomationId = "tab-messages";
                    tab.AutomationName = "Messages Tab";
                }),
                PrismCustomTabs.CreateTab<ProfilePage>(container, "profile", "P", tab =>
                {
                    tab.Title = "Profile";
                    tab.TitleProvider = () => localizationService.Translate("Profile");
                    tab.AutomationId = "tab-profile";
                    tab.AutomationName = "Profile Tab";
                }),
                PrismCustomTabs.CreateTab<SettingsPage>(container, "settings", "G", tab =>
                {
                    tab.Title = "Settings";
                    tab.TitleProvider = () => localizationService.Translate("Settings");
                    tab.AutomationId = "tab-settings";
                    tab.AutomationName = "Settings Tab";
                })
            };

            return PrismCustomTabs.CreateViewModel(tabs, "home", options);
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

    private static void ConfigureOptions(CustomTabsOptions options)
    {
        if (_optionsConfigured)
        {
            return;
        }

        _optionsConfigured = true;
        options.BackgroundColor = Colors.DarkBlue;
        options.PageBackgroundColor = Colors.Blue;
        options.ContentBackgroundColor = Colors.Green;
        options.AccentColor = Color.FromArgb("#E5E7EB");
        options.SelectedIconColor = Color.FromArgb("#F9FAFB");
        options.UnselectedIconColor = Color.FromArgb("#6B7280");
        options.SelectedTextColor = Color.FromArgb("#F9FAFB");
        options.UnselectedTextColor = Color.FromArgb("#9CA3AF");
        options.ShowText = true;
        options.EnableAnimations = true;
        options.EnableHaptics = false;
        options.TabBarHeight = 52;
        options.TabBarPadding = new Thickness(8, 4, 8, 6);
        options.IconSize = 22;
        options.TextSize = 11;
        options.TabLayoutMode = TabLayoutMode.Fixed;
        options.ScrollableThreshold = int.MaxValue;
        options.BorderColor = Color.FromArgb("#374151");
        options.BorderThickness = 1;
        options.UnderlineMargin = new Thickness(0, 6, 0, 0);
        options.ReselectBehavior = TabReselectBehavior.PopToRoot | TabReselectBehavior.ScrollToTop;
        options.VisualStyle = TabVisualStyle.ClassicBottom;

        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            options.BadgeOffsetY = 0;
        }
    }

    private static void ConfigureBadge(CustomTabBadge badge)
    {
        if (_badgeConfigured)
        {
            return;
        }

        _badgeConfigured = true;
        badge.Count = 2;
        badge.BackgroundColor = Colors.Red;
        badge.TextColor = Colors.White;
    }
}
