using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Plugin.Maui.CustomTabs.Animations;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Services;
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

    public static readonly BindableProperty EffectiveShowTextProperty = BindableProperty.Create(
        nameof(EffectiveShowText),
        typeof(bool),
        typeof(CustomTabBarView),
        true);

    public static readonly BindableProperty EffectiveShowIconsProperty = BindableProperty.Create(
        nameof(EffectiveShowIcons),
        typeof(bool),
        typeof(CustomTabBarView),
        true);

    public static readonly BindableProperty EffectiveShowUnderlineProperty = BindableProperty.Create(
        nameof(EffectiveShowUnderline),
        typeof(bool),
        typeof(CustomTabBarView),
        true);

    public static readonly BindableProperty SafeAreaInsetsProperty = BindableProperty.Create(
        nameof(SafeAreaInsets),
        typeof(Thickness),
        typeof(CustomTabBarView),
        default(Thickness),
        propertyChanged: OnSafeAreaInsetsChanged);

    public static readonly BindableProperty EffectiveItemPaddingProperty = BindableProperty.Create(
        nameof(EffectiveItemPadding),
        typeof(Thickness),
        typeof(CustomTabBarView),
        new Thickness(8, 4));

    public static readonly BindableProperty EffectiveScrollableItemMarginProperty = BindableProperty.Create(
        nameof(EffectiveScrollableItemMargin),
        typeof(Thickness),
        typeof(CustomTabBarView),
        new Thickness(12, 0));

    public static readonly BindableProperty EffectiveItemCornerRadiusProperty = BindableProperty.Create(
        nameof(EffectiveItemCornerRadius),
        typeof(float),
        typeof(CustomTabBarView),
        0f);

    public static readonly BindableProperty EffectiveItemBorderColorProperty = BindableProperty.Create(
        nameof(EffectiveItemBorderColor),
        typeof(Color),
        typeof(CustomTabBarView),
        Colors.Transparent);

    public static readonly BindableProperty EffectiveItemBorderThicknessProperty = BindableProperty.Create(
        nameof(EffectiveItemBorderThickness),
        typeof(double),
        typeof(CustomTabBarView),
        0d);

    public static readonly BindableProperty EffectiveSelectedItemBackgroundColorProperty = BindableProperty.Create(
        nameof(EffectiveSelectedItemBackgroundColor),
        typeof(Color),
        typeof(CustomTabBarView),
        Colors.Transparent);

    public static readonly BindableProperty EffectiveUnselectedItemBackgroundColorProperty = BindableProperty.Create(
        nameof(EffectiveUnselectedItemBackgroundColor),
        typeof(Color),
        typeof(CustomTabBarView),
        Colors.Transparent);

    public static readonly BindableProperty EffectiveTabBarHeightProperty = BindableProperty.Create(
        nameof(EffectiveTabBarHeight),
        typeof(double),
        typeof(CustomTabBarView),
        76d);

    public static readonly BindableProperty EffectiveTabBarMinHeightProperty = BindableProperty.Create(
        nameof(EffectiveTabBarMinHeight),
        typeof(double),
        typeof(CustomTabBarView),
        56d);

    public static readonly BindableProperty EffectiveTabBarPaddingProperty = BindableProperty.Create(
        nameof(EffectiveTabBarPadding),
        typeof(Thickness),
        typeof(CustomTabBarView),
        new Thickness(0, 10, 0, 16));

    public static readonly BindableProperty EffectiveTabBarCornerRadiusProperty = BindableProperty.Create(
        nameof(EffectiveTabBarCornerRadius),
        typeof(CornerRadius),
        typeof(CustomTabBarView),
        default(CornerRadius));

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

    public bool EffectiveShowText
    {
        get => (bool)GetValue(EffectiveShowTextProperty);
        set => SetValue(EffectiveShowTextProperty, value);
    }

    public bool EffectiveShowIcons
    {
        get => (bool)GetValue(EffectiveShowIconsProperty);
        set => SetValue(EffectiveShowIconsProperty, value);
    }

    public bool EffectiveShowUnderline
    {
        get => (bool)GetValue(EffectiveShowUnderlineProperty);
        set => SetValue(EffectiveShowUnderlineProperty, value);
    }

    public Thickness SafeAreaInsets
    {
        get => (Thickness)GetValue(SafeAreaInsetsProperty);
        set => SetValue(SafeAreaInsetsProperty, value);
    }

    public Thickness EffectiveItemPadding
    {
        get => (Thickness)GetValue(EffectiveItemPaddingProperty);
        set => SetValue(EffectiveItemPaddingProperty, value);
    }

    public Thickness EffectiveScrollableItemMargin
    {
        get => (Thickness)GetValue(EffectiveScrollableItemMarginProperty);
        set => SetValue(EffectiveScrollableItemMarginProperty, value);
    }

    public float EffectiveItemCornerRadius
    {
        get => (float)GetValue(EffectiveItemCornerRadiusProperty);
        set => SetValue(EffectiveItemCornerRadiusProperty, value);
    }

    public Color EffectiveItemBorderColor
    {
        get => (Color)GetValue(EffectiveItemBorderColorProperty);
        set => SetValue(EffectiveItemBorderColorProperty, value);
    }

    public double EffectiveItemBorderThickness
    {
        get => (double)GetValue(EffectiveItemBorderThicknessProperty);
        set => SetValue(EffectiveItemBorderThicknessProperty, value);
    }

    public Color EffectiveSelectedItemBackgroundColor
    {
        get => (Color)GetValue(EffectiveSelectedItemBackgroundColorProperty);
        set => SetValue(EffectiveSelectedItemBackgroundColorProperty, value);
    }

    public Color EffectiveUnselectedItemBackgroundColor
    {
        get => (Color)GetValue(EffectiveUnselectedItemBackgroundColorProperty);
        set => SetValue(EffectiveUnselectedItemBackgroundColorProperty, value);
    }

    public double EffectiveTabBarHeight
    {
        get => (double)GetValue(EffectiveTabBarHeightProperty);
        set => SetValue(EffectiveTabBarHeightProperty, value);
    }

    public double EffectiveTabBarMinHeight
    {
        get => (double)GetValue(EffectiveTabBarMinHeightProperty);
        set => SetValue(EffectiveTabBarMinHeightProperty, value);
    }

    public Thickness EffectiveTabBarPadding
    {
        get => (Thickness)GetValue(EffectiveTabBarPaddingProperty);
        set => SetValue(EffectiveTabBarPaddingProperty, value);
    }

    public CornerRadius EffectiveTabBarCornerRadius
    {
        get => (CornerRadius)GetValue(EffectiveTabBarCornerRadiusProperty);
        set => SetValue(EffectiveTabBarCornerRadiusProperty, value);
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
        ApplyVisualStyle();
        ApplyItemTemplate();
        UpdateLayoutMode();
    }

    private static void OnViewModelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomTabBarView)bindable;
        control.ApplyViewModel(newValue as CustomTabsViewModel, fromProperty: true);
    }

    private static void OnSafeAreaInsetsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomTabBarView)bindable;
        control.ApplyVisualStyle();
        control.UpdateLayoutMode();
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
        ApplyVisualStyle();

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
        var height = EffectiveTabBarHeight;
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

    private void ApplyVisualStyle()
    {
        var options = _viewModel?.Options;
        if (options == null)
        {
            return;
        }

        var style = options.VisualStyle;
        EffectiveShowIcons = style != TabVisualStyle.Minimal;
        EffectiveShowText = style != TabVisualStyle.IconOnly && options.ShowText;
        EffectiveShowUnderline = style != TabVisualStyle.Segmented
            && style != TabVisualStyle.Pills
            && options.ShowUnderline;

        // Never allow a fully blank tab item when style + toggles conflict.
        if (!EffectiveShowIcons && !EffectiveShowText)
        {
            EffectiveShowText = true;
        }

        EffectiveItemPadding = style switch
        {
            TabVisualStyle.IconOnly => new Thickness(8, 8),
            TabVisualStyle.Compact => new Thickness(6, 2),
            TabVisualStyle.Segmented => new Thickness(10, 6),
            TabVisualStyle.Pills => new Thickness(10, 6),
            TabVisualStyle.Minimal => new Thickness(12, 6),
            _ => new Thickness(8, 4)
        };

        EffectiveScrollableItemMargin = style switch
        {
            TabVisualStyle.Compact => new Thickness(6, 0),
            TabVisualStyle.Segmented => new Thickness(8, 0),
            TabVisualStyle.Pills => new Thickness(8, 0),
            _ => new Thickness(12, 0)
        };

        EffectiveItemCornerRadius = style switch
        {
            TabVisualStyle.Segmented => 16f,
            TabVisualStyle.Pills => 18f,
            _ => 0f
        };

        EffectiveItemBorderThickness = style switch
        {
            TabVisualStyle.Segmented => 1,
            TabVisualStyle.Pills => 1,
            _ => 0
        };

        EffectiveItemBorderColor = style switch
        {
            TabVisualStyle.Segmented => options.BorderColor == Colors.Transparent ? options.AccentColor.WithAlpha(0.25f) : options.BorderColor,
            TabVisualStyle.Pills => options.BorderColor == Colors.Transparent ? options.AccentColor.WithAlpha(0.20f) : options.BorderColor,
            _ => Colors.Transparent
        };

        EffectiveSelectedItemBackgroundColor = style switch
        {
            TabVisualStyle.Segmented => options.AccentColor.WithAlpha(0.22f),
            TabVisualStyle.Pills => options.AccentColor.WithAlpha(0.18f),
            _ => Colors.Transparent
        };

        EffectiveUnselectedItemBackgroundColor = Colors.Transparent;

        var requestedHeight = style switch
        {
            TabVisualStyle.Compact => Math.Max(56, options.TabBarHeight - 8),
            _ => options.TabBarHeight
        };

        EffectiveTabBarPadding = style switch
        {
            TabVisualStyle.IconOnly => new Thickness(0, 8, 0, 12),
            TabVisualStyle.Compact => new Thickness(0, 6, 0, 10),
            TabVisualStyle.Segmented => new Thickness(8, 8, 8, 8),
            TabVisualStyle.Pills => new Thickness(8, 8, 8, 8),
            _ => options.TabBarPadding
        };

        var safeInsets = SafeAreaInsets;
        if (safeInsets != default)
        {
            if (style == TabVisualStyle.TopUnderline)
            {
                EffectiveTabBarPadding = new Thickness(
                    EffectiveTabBarPadding.Left,
                    EffectiveTabBarPadding.Top + safeInsets.Top,
                    EffectiveTabBarPadding.Right,
                    EffectiveTabBarPadding.Bottom);
            }
            else
            {
                EffectiveTabBarPadding = new Thickness(
                    EffectiveTabBarPadding.Left,
                    EffectiveTabBarPadding.Top,
                    EffectiveTabBarPadding.Right,
                    EffectiveTabBarPadding.Bottom + safeInsets.Bottom);
            }
        }

        EffectiveTabBarCornerRadius = style switch
        {
            TabVisualStyle.Floating => WithFallbackCornerRadius(options.TabBarCornerRadius, 22),
            TabVisualStyle.Segmented => WithFallbackCornerRadius(options.TabBarCornerRadius, 18),
            TabVisualStyle.Pills => WithFallbackCornerRadius(options.TabBarCornerRadius, 20),
            _ => options.TabBarCornerRadius
        };

        // Keep enough room for icon/text/indicator + padding so items never clip.
        var contentHeight = 0d;
        if (EffectiveShowIcons)
        {
            contentHeight += options.IconSize;
        }

        if (EffectiveShowText)
        {
            contentHeight += options.TextSize + 4;
        }

        if (EffectiveShowUnderline)
        {
            contentHeight += options.UnderlineHeight + options.UnderlineMargin.Top + options.UnderlineMargin.Bottom;
        }

        var requiredMinHeight = EffectiveTabBarPadding.Top + contentHeight + EffectiveTabBarPadding.Bottom + 10;
        var styleMinHeight = style == TabVisualStyle.Compact ? 52d : 56d;
        EffectiveTabBarMinHeight = Math.Max(styleMinHeight, requiredMinHeight);
        EffectiveTabBarHeight = Math.Max(requestedHeight, EffectiveTabBarMinHeight);
    }

    private static CornerRadius WithFallbackCornerRadius(CornerRadius radius, double fallback)
    {
        if (radius.TopLeft > 0 || radius.TopRight > 0 || radius.BottomRight > 0 || radius.BottomLeft > 0)
        {
            return radius;
        }

        return new CornerRadius(fallback);
    }

    private void OnTabViewLoaded(object sender, EventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, "CustomTabBarView.OnTabViewLoaded");
        }
    }

    private void OnTabViewUnloaded(object sender, EventArgs e)
    {
        try
        {
            if (sender is View element && element.BindingContext is CustomTabItem tab)
            {
                _tabVisuals.Remove(tab);
            }
        }
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, "CustomTabBarView.OnTabViewUnloaded");
        }
    }

    private async void OnTabTapped(object? sender, TappedEventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, "CustomTabBarView.OnTabTapped");
        }
    }

    private sealed record TabVisuals(VisualElement Icon, VisualElement? Underline);
}
