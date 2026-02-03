using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Plugin.Maui.CustomTabs.Animations;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Themes;
using Plugin.Maui.CustomTabs.ViewModels;

namespace Plugin.Maui.CustomTabs.Controls;

/// <summary>
/// Custom tab bar control that renders tabs from the view model.
/// </summary>
public partial class CustomTabBarView : ContentView
{
    private readonly Dictionary<CustomTabItem, TabVisuals> _tabVisuals = new();

    /// <summary>
    /// Creates the control.
    /// </summary>
    public CustomTabBarView()
    {
        EnsureResources();
        InitializeComponent();
    }

    private CustomTabsViewModel? ViewModel => BindingContext as CustomTabsViewModel;

    private static void EnsureResources()
    {
        var resources = Application.Current?.Resources;
        if (resources == null)
        {
            return;
        }

        if (!resources.MergedDictionaries.OfType<CustomTabsResources>().Any())
        {
            resources.MergedDictionaries.Add(new CustomTabsResources());
        }
    }

    private void OnTabViewLoaded(object sender, EventArgs e)
    {
        if (sender is not Grid grid || grid.BindingContext is not CustomTabItem tab)
        {
            return;
        }

        var icon = grid.FindByName<VisualElement>("IconContainer");
        var underline = grid.FindByName<VisualElement>("Underline");

        if (icon != null)
        {
            _tabVisuals[tab] = new TabVisuals(icon, underline);
        }
    }

    private void OnTabViewUnloaded(object sender, EventArgs e)
    {
        if (sender is Grid grid && grid.BindingContext is CustomTabItem tab)
        {
            _tabVisuals.Remove(tab);
        }
    }

    private async void OnTabTapped(object sender, TappedEventArgs e)
    {
        var tab = e.Parameter as CustomTabItem ?? (sender as Grid)?.BindingContext as CustomTabItem;
        var viewModel = ViewModel;
        if (tab == null || viewModel == null)
        {
            return;
        }

        if (viewModel.Options.EnableHaptics)
        {
            TabAnimations.TryPerformHaptic();
        }

        viewModel.SelectTabCommand.Execute(tab);

        if (viewModel.Options.EnableAnimations && _tabVisuals.TryGetValue(tab, out var visuals))
        {
            await TabAnimations.AnimateSelectionAsync(visuals.Icon, visuals.Underline, viewModel.Options.AnimationDuration);
        }
    }

    private sealed record TabVisuals(VisualElement Icon, VisualElement? Underline);
}
