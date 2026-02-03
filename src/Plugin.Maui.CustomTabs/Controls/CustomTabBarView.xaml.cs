using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Maui.Controls;
using Plugin.Maui.CustomTabs.Animations;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Themes;
using Plugin.Maui.CustomTabs.ViewModels;
using System.Diagnostics;

namespace Plugin.Maui.CustomTabs.Controls;

/// <summary>
/// Custom tab bar control that renders tabs from the view model.
/// </summary>
public partial class CustomTabBarView : ContentView
{
    public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
        nameof(ViewModel),
        typeof(CustomTabsViewModel),
        typeof(CustomTabBarView),
        default(CustomTabsViewModel),
        propertyChanged: OnViewModelChanged);

    private readonly Dictionary<CustomTabItem, TabVisuals> _tabVisuals = new();
    private CustomTabsViewModel? _viewModel;
    private BindableObject? _parentBindingSource;
    private bool _suppressViewModelSync;

    /// <summary>
    /// View model used by the tab bar. Setting this keeps the BindingContext in sync.
    /// </summary>
    public CustomTabsViewModel? ViewModel
    {
        get => (CustomTabsViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Creates the control.
    /// </summary>
    public CustomTabBarView()
    {
        EnsureResources();
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        var typeName = BindingContext?.GetType().Name ?? "null";
        Debug.WriteLine($"[CustomTabs] CustomTabBarView OnBindingContextChanged: {typeName}.");
        if (_suppressViewModelSync)
        {
            return;
        }

        var viewModel = BindingContext as CustomTabsViewModel;
        ApplyViewModel(viewModel, fromProperty: false);
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        HookParentBindingContext();
    }

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

    private void HookViewModel(CustomTabsViewModel? viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.Options.PropertyChanged -= OnOptionsPropertyChanged;
            _viewModel.Tabs.CollectionChanged -= OnTabsCollectionChanged;
        }

        _viewModel = viewModel;

        if (_viewModel != null)
        {
            _viewModel.Options.PropertyChanged += OnOptionsPropertyChanged;
            _viewModel.Tabs.CollectionChanged += OnTabsCollectionChanged;
        }

        var tabCount = _viewModel?.Tabs.Count ?? 0;
        Debug.WriteLine($"[CustomTabs] CustomTabBarView HookViewModel applied. Tabs={tabCount}.");
        ApplyItemTemplate();
        UpdateLayoutMode();
    }

    private static void OnViewModelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomTabBarView)bindable;
        control.ApplyViewModel(newValue as CustomTabsViewModel, fromProperty: true);
    }

    private void ApplyViewModel(CustomTabsViewModel? viewModel, bool fromProperty)
    {
        if (ReferenceEquals(_viewModel, viewModel))
        {
            if (fromProperty && !ReferenceEquals(BindingContext, viewModel))
            {
                _suppressViewModelSync = true;
                BindingContext = viewModel;
                _suppressViewModelSync = false;
            }

            return;
        }

        HookViewModel(viewModel);

        if (fromProperty)
        {
            if (!ReferenceEquals(BindingContext, viewModel))
            {
                _suppressViewModelSync = true;
                BindingContext = viewModel;
                _suppressViewModelSync = false;
            }
        }
        else
        {
            _suppressViewModelSync = true;
            ViewModel = viewModel;
            _suppressViewModelSync = false;
        }
    }

    private void HookParentBindingContext()
    {
        if (_parentBindingSource != null)
        {
            _parentBindingSource.BindingContextChanged -= OnParentBindingContextChanged;
        }

        _parentBindingSource = Parent as BindableObject;
        if (_parentBindingSource != null)
        {
            _parentBindingSource.BindingContextChanged += OnParentBindingContextChanged;
            if (BindingContext == null && _parentBindingSource.BindingContext is CustomTabsViewModel viewModel)
            {
                ViewModel = viewModel;
            }
        }
    }

    private void OnParentBindingContextChanged(object? sender, EventArgs e)
    {
        if (BindingContext is CustomTabsViewModel)
        {
            return;
        }

        if (_parentBindingSource?.BindingContext is CustomTabsViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }

    private void OnOptionsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CustomTabsOptions.TabItemTemplate))
        {
            ApplyItemTemplate();
        }

        if (e.PropertyName == nameof(CustomTabsOptions.TabLayoutMode)
            || e.PropertyName == nameof(CustomTabsOptions.ScrollableThreshold))
        {
            UpdateLayoutMode();
        }
    }

    private void OnTabsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateLayoutMode();
    }

    private void ApplyItemTemplate()
        => ApplyItemTemplate(false);

    private void ApplyItemTemplate(bool useScrollable)
    {
        var template = ResolveItemTemplate(useScrollable);
        if (template == null)
        {
            Debug.WriteLine("[CustomTabs] CustomTabBarView ApplyItemTemplate: no template resolved.");
            return;
        }

        BindableLayout.SetItemTemplate(FixedTabLayout, template);
        BindableLayout.SetItemTemplate(ScrollableTabLayout, template);
        Debug.WriteLine("[CustomTabs] CustomTabBarView ApplyItemTemplate: template applied.");
    }

    private DataTemplate? ResolveItemTemplate(bool useScrollable)
    {
        var optionsTemplate = _viewModel?.Options.TabItemTemplate;
        if (optionsTemplate != null)
        {
            return optionsTemplate;
        }

        var key = useScrollable ? "ScrollableTabTemplate" : "DefaultTabTemplate";
        if (Resources.TryGetValue(key, out var resource) && resource is DataTemplate template)
        {
            return template;
        }

        return null;
    }

    private void UpdateLayoutMode()
    {
        var viewModel = _viewModel;
        if (viewModel == null)
        {
            Debug.WriteLine("[CustomTabs] CustomTabBarView UpdateLayoutMode: no view model.");
            return;
        }

        var options = viewModel.Options;
        var tabCount = viewModel.Tabs.Count;
        var height = options.TabBarHeight;
        var useScrollable = options.TabLayoutMode == TabLayoutMode.Scrollable
            || (options.TabLayoutMode == TabLayoutMode.Auto && tabCount > options.ScrollableThreshold);

        ApplyItemTemplate(useScrollable);

        if (useScrollable)
        {
            FixedTabLayout.IsVisible = false;
            ScrollableTabHost.IsVisible = true;
            BindableLayout.SetItemsSource(ScrollableTabLayout, viewModel.Tabs);
            BindableLayout.SetItemsSource(FixedTabLayout, null);
        }
        else
        {
            FixedTabLayout.IsVisible = true;
            ScrollableTabHost.IsVisible = false;
            BindableLayout.SetItemsSource(FixedTabLayout, viewModel.Tabs);
            BindableLayout.SetItemsSource(ScrollableTabLayout, null);
        }

        _tabVisuals.Clear();
        Debug.WriteLine($"[CustomTabs] CustomTabBarView UpdateLayoutMode: tabs={tabCount}, scrollable={useScrollable}, height={height}.");
    }

    private void OnTabViewLoaded(object sender, EventArgs e)
    {
        if (sender is not View element || element.BindingContext is not CustomTabItem tab)
        {
            return;
        }

        if (!element.GestureRecognizers.OfType<TapGestureRecognizer>().Any())
        {
            var tap = new TapGestureRecognizer { CommandParameter = tab };
            tap.Tapped += OnTabTapped;
            element.GestureRecognizers.Add(tap);
        }

        var icon = element.FindByName<VisualElement>("IconContainer");
        var underline = element.FindByName<VisualElement>("Underline");

        if (icon != null)
        {
            _tabVisuals[tab] = new TabVisuals(icon, underline);
        }
    }

    private void OnTabViewUnloaded(object sender, EventArgs e)
    {
        if (sender is View element && element.BindingContext is CustomTabItem tab)
        {
            _tabVisuals.Remove(tab);
        }
    }

    private async void OnTabTapped(object? sender, TappedEventArgs e)
    {
        var tab = e.Parameter as CustomTabItem ?? (sender as BindableObject)?.BindingContext as CustomTabItem;
        var viewModel = ViewModel;
        if (tab == null || viewModel == null)
        {
            return;
        }

        if (!tab.IsEnabled || !tab.IsVisible)
        {
            return;
        }

        if (viewModel.Options.EnableHaptics)
        {
            TabAnimations.TryPerformHaptic();
        }

        var result = viewModel.TrySelectTab(tab, TabSelectionOrigin.User, allowReselect: true);
        if (result == TabSelectionResult.Blocked || result == TabSelectionResult.None)
        {
            return;
        }

        if (result == TabSelectionResult.Reselected)
        {
            await viewModel.HandleReselectAsync(tab);
        }

        if (viewModel.Options.EnableAnimations && _tabVisuals.TryGetValue(tab, out var visuals))
        {
            await TabAnimations.AnimateSelectionAsync(
                visuals.Icon,
                visuals.Underline,
                viewModel.Options.AnimationDuration,
                viewModel.Options.IconAnimationStyle,
                viewModel.Options.IndicatorAnimationStyle);
        }
    }

    private sealed record TabVisuals(VisualElement Icon, VisualElement? Underline);
}
