using CustomTabs.Prism.Sample.Pages;
using CustomTabs.Prism.Sample.Services;
using CustomTabs.Prism.Sample.ViewModels;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.CustomTabs.Extensions;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Services;
using Prism;
using Prism.Ioc;

namespace CustomTabs.Prism.Sample;

/// <summary>
/// Maui app builder for the Prism sample app.
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

        builder.Services.AddSingleton<SampleExceptionHandler>();
        builder.Services.AddSingleton<ICustomTabsExceptionHandler>(sp => sp.GetRequiredService<SampleExceptionHandler>());

        builder.UsePrism(prism =>
        {
            prism.RegisterTypes(container =>
            {
                container.RegisterSingleton<CustomTabsOptions>();

                container.RegisterForNavigation<HomePage, HomePageViewModel>();
                container.RegisterForNavigation<SearchPage, SearchPageViewModel>();
                container.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
                container.RegisterForNavigation<DetailPage, DetailPageViewModel>();
                container.RegisterForNavigation<MainTabsPage>();
            })
            .OnInitialized(() =>
            {
                // Keep Prism warm; window creation is handled by App.CreateWindow.
            });
        });

        return builder.Build();
    }
}
