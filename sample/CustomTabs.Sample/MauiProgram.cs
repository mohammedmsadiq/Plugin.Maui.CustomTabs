using CustomTabs.Sample.Pages;
using CustomTabs.Sample.Services;
using CustomTabs.Sample.ViewModels;
using System.Linq;
using System.Diagnostics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.CustomTabs.Extensions;
using Plugin.Maui.CustomTabs.Services;
using Plugin.Maui.CustomTabs.Models;
using Prism;
using Prism.Ioc;

namespace CustomTabs.Sample;

/// <summary>
/// Maui app builder for the sample app.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// Creates the Maui app.
    /// </summary>
    public static MauiApp CreateMauiApp()
    {
        Debug.WriteLine("[Sample] MauiProgram.CreateMauiApp starting.");
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseCustomTabs();

        builder.Services.AddSingleton<SampleExceptionHandler>();
        builder.Services.AddSingleton<ICustomTabsExceptionHandler>(sp => sp.GetRequiredService<SampleExceptionHandler>());

        builder.UsePrism(prism =>
        {
            prism.RegisterTypes(container =>
            {
                Debug.WriteLine("[Sample] RegisterTypes starting.");
                container.RegisterSingleton<SimpleLocalizationService>();
                container.RegisterSingleton<IDemoAuthService, DemoAuthService>();
                container.RegisterSingleton<CustomTabsOptions>();
                container.RegisterSingleton<CustomTabBadge>();

                container.RegisterForNavigation<SplashPage, SplashPageViewModel>();
                container.RegisterForNavigation<LoginPage, LoginPageViewModel>();
                container.RegisterForNavigation<HomePage, HomePageViewModel>();
                container.RegisterForNavigation<SearchPage, SearchPageViewModel>();
                container.RegisterForNavigation<MessagesPage, MessagesPageViewModel>();
                container.RegisterForNavigation<ProfilePage, ProfilePageViewModel>();
                container.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
                container.RegisterForNavigation<DetailPage, DetailPageViewModel>();
                container.RegisterForNavigation<MainTabsPage>();
                Debug.WriteLine("[Sample] RegisterTypes complete.");
            })
            .OnInitialized(() =>
            {
                Debug.WriteLine("[Sample] Prism initialized.");
            });
        });

        Debug.WriteLine("[Sample] MauiProgram.CreateMauiApp building.");
        return builder.Build();
    }
}
