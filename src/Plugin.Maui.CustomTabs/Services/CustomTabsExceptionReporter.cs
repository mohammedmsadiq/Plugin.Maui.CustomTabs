using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Maui.CustomTabs.Services;

internal static class CustomTabsExceptionReporter
{
    public static void Report(Exception exception, string context)
    {
        try
        {
            var services = Application.Current?.Handler?.MauiContext?.Services;
            if (services == null)
            {
                Debug.WriteLine($"[CustomTabs][Error] {context}: {exception}");
                return;
            }

            var handler = services.GetService<ICustomTabsExceptionHandler>();
            if (handler != null)
            {
                handler.Handle(exception, context);
                return;
            }
        }
        catch (Exception resolutionError)
        {
            Debug.WriteLine($"[CustomTabs][Error] Failed to resolve exception handler: {resolutionError}");
        }

        Debug.WriteLine($"[CustomTabs][Error] {context}: {exception}");
    }
}
