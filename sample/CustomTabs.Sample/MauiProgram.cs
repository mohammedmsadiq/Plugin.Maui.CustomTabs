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

                container.RegisterForNavigation<SplashPage, SplashPageViewModel>();
                container.RegisterForNavigation<LoginPage, LoginPageViewModel>();
                container.RegisterForNavigation<MainTabsPage>();
                Debug.WriteLine("[Sample] RegisterTypes complete.");
            })
            .CreateWindow(async (container, navigation) =>
            {
                Debug.WriteLine("[Sample] Prism CreateWindow invoked.");
                // Keep startup deterministic: land directly on tabs.
                // Splash remains registered for manual/testing navigation.
                var result = await navigation.NavigateAsync("/MainTabsPage");
                if (!result.Success)
                {
                    SafeExecution.Run(() =>
                    {
                        System.Diagnostics.Debug.WriteLine(result.Exception);
                        var errorPage = new ContentPage
                        {
                            Content = new ScrollView
                            {
                                Content = new Label
                                {
                                    Text = result.Exception?.ToString() ?? "Navigation failed.",
                                    Margin = new Thickness(24)
                                }
                            }
                        };

                        var window = Application.Current?.Windows.FirstOrDefault();
                        if (window != null)
                        {
                            window.Page = errorPage;
                        }
                    }, "MauiProgram.CreateWindow.NavigationFailureFallback");
                }
                else
                {
                    Debug.WriteLine("[Sample] Initial navigation succeeded.");
                }
            });
        });

        Debug.WriteLine("[Sample] MauiProgram.CreateMauiApp building.");
        return builder.Build();
    }
}
