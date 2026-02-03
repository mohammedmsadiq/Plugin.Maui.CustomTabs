using CustomTabs.Sample.Pages;
using CustomTabs.Sample.Services;
using CustomTabs.Sample.ViewModels;
using System.Diagnostics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
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
        Debug.WriteLine("[Sample] MauiProgram.CreateMauiApp starting.");
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseCustomTabs();

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
                var result = await navigation.NavigateAsync("/NavigationPage/SplashPage");
                if (!result.Success)
                {
                    System.Diagnostics.Debug.WriteLine(result.Exception);
                    Application.Current!.MainPage = new ContentPage
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
