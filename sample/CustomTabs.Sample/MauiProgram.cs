using CustomTabs.Sample.Pages;
using CustomTabs.Sample.Services;
using CustomTabs.Sample.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Plugin.Maui.CustomTabs.Extensions;
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
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseCustomTabs();

        builder.UsePrism(prism =>
        {
            prism.RegisterTypes(container =>
            {
                container.RegisterSingleton<SimpleLocalizationService>();
                container.RegisterSingleton<IDemoAuthService, DemoAuthService>();

                container.RegisterForNavigation<SplashPage, SplashPageViewModel>();
                container.RegisterForNavigation<LoginPage, LoginPageViewModel>();
                container.RegisterForNavigation<MainTabsPage>();
            })
            .CreateWindow(navigation => navigation.NavigateAsync("/SplashPage"));
        });

        return builder.Build();
    }
}
