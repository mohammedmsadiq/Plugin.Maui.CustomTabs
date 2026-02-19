using Microsoft.Maui.Controls;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.ViewModels;
using Prism.Controls;
using Prism.Ioc;
using Prism.Navigation.Xaml;

namespace Plugin.Maui.CustomTabs.Prism;

/// <summary>
/// Prism helper utilities for building custom tab items.
/// </summary>
public static class PrismCustomTabs
{
    /// <summary>
    /// Creates a tab item whose root page is resolved from Prism and hosted in a PrismNavigationPage.
    /// </summary>
    public static CustomTabItem CreateTab<TPage>(
        IContainerProvider container,
        string key,
        string iconGlyph,
        Action<CustomTabItem>? configure = null)
        where TPage : Page
    {
        if (container is null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        var item = new CustomTabItem(key, iconGlyph, () =>
        {
            var page = container.Resolve<TPage>();
            TrySetContainerProvider(page, container);
            return page;
        })
        {
            NavigationPageFactory = rootPage =>
            {
                var nav = new PrismNavigationPage(rootPage);
                TrySetContainerProvider(nav, container);
                return nav;
            }
        };

        configure?.Invoke(item);
        return item;
    }

    /// <summary>
    /// Creates a view model for the custom tabs host page.
    /// </summary>
    public static CustomTabsViewModel CreateViewModel(
        IEnumerable<CustomTabItem> tabs,
        string defaultKey,
        CustomTabsOptions options)
    {
        if (tabs is null)
        {
            throw new ArgumentNullException(nameof(tabs));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return new CustomTabsViewModel(tabs, defaultKey, options);
    }

    /// <summary>
    /// Attempts to set the Prism container provider on a bindable object.
    /// Supports multiple Prism signatures for compatibility.
    /// </summary>
    public static void TrySetContainerProvider(BindableObject bindable, IContainerProvider container)
    {
        if (bindable is null)
        {
            return;
        }

        if (container is null)
        {
            return;
        }

        var navigationType = typeof(Navigation);
        var method = navigationType.GetMethod(
            "SetContainerProvider",
            new[] { typeof(BindableObject), typeof(IContainerProvider) });
        if (method != null)
        {
            method.Invoke(null, new object[] { bindable, container });
            return;
        }

        method = navigationType.GetMethod(
            "SetContainerProvider",
            new[] { typeof(IContainerProvider) });
        if (method != null)
        {
            method.Invoke(null, new object[] { container });
        }
    }
}
