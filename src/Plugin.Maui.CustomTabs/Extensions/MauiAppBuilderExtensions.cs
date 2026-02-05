using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Maui.CustomTabs.Services;
using Plugin.Maui.CustomTabs.Themes;

namespace Plugin.Maui.CustomTabs.Extensions;

/// <summary>
/// Maui builder extensions for the custom tabs library.
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Registers resources required by the custom tabs components.
    /// </summary>
    public static MauiAppBuilder UseCustomTabs(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IMauiInitializeService, CustomTabsInitializer>();
        builder.Services.TryAddSingleton<ICustomTabsExceptionHandler, DebugCustomTabsExceptionHandler>();
        return builder;
    }

    private sealed class CustomTabsInitializer : IMauiInitializeService
    {
        public void Initialize(IServiceProvider services)
        {
            if (Application.Current?.Resources == null)
            {
                return;
            }

            var dictionaries = Application.Current.Resources.MergedDictionaries;
            if (!dictionaries.OfType<CustomTabsResources>().Any())
            {
                dictionaries.Add(new CustomTabsResources());
            }
        }
    }
}
