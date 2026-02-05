using Microsoft.Maui.Controls.PlatformConfiguration;
#if IOS
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif
using Microsoft.Maui.Devices;
using Microsoft.Maui;
using Plugin.Maui.CustomTabs.Controls;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Services;
using Plugin.Maui.CustomTabs.ViewModels;
using System.Diagnostics;
#if IOS
using UIKit;
#endif
#if ANDROID
using AndroidX.Core.View;
using Microsoft.Maui.ApplicationModel;
#endif
#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.System;
#endif

namespace Plugin.Maui.CustomTabs.Pages;

/// <summary>
/// Host page that renders the selected tab content with a custom tab bar.
/// </summary>
public partial class CustomTabsHostPage : ContentPage
{
    private readonly ILocalizationRefreshService? _localizationService;
    private bool _localizationSubscribed;
    private CustomTabsViewModel? _subscribedViewModel;
    private bool _suppressSelectionSync;
    private Func<CustomTabItem, bool>? _canSelectTab;
    private bool _layoutLogged;
#if WINDOWS
    private bool _keyboardAttached;
#endif

    /// <summary>
    /// Creates a new host page.
    /// </summary>
    public CustomTabsHostPage(CustomTabsViewModel viewModel, ILocalizationRefreshService? localizationService)
    {
        Debug.WriteLine("[CustomTabs] CustomTabsHostPage constructor invoked.");
        InitializeComponent();
#if IOS
        this.On<iOS>().SetUseSafeArea(false);
#endif
        Padding = Thickness.Zero;
        BindingContext = viewModel;
        _localizationService = localizationService;
        Loaded += OnHostLoaded;

        ApplyTabBarBindingContext(viewModel);
        ApplyTabBarPlacement();
        SubscribeViewModel();
        SyncSelectionFromViewModel();

        UpdateSafeAreaPadding();
    }

    /// <summary>
    /// Bindable selected tab key.
    /// </summary>
    public string? SelectedTabKey
    {
        get => (string?)GetValue(SelectedTabKeyProperty);
        set => SetValue(SelectedTabKeyProperty, value);
    }

    /// <summary>
    /// Bindable selected tab index.
    /// </summary>
    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// Optional predicate to allow or block selection.
    /// </summary>
    public Func<CustomTabItem, bool>? CanSelectTab
    {
        get => _canSelectTab;
        set
        {
            _canSelectTab = value;
            if (ViewModel != null)
            {
                ViewModel.CanSelectTab = value;
            }
        }
    }

    /// <summary>
    /// Raised before a tab selection occurs. Set Cancel to true to block.
    /// </summary>
    public event EventHandler<TabSelectionRequestedEventArgs>? TabSelectionRequested;

    /// <summary>
    /// Raised when the selected tab changes.
    /// </summary>
    public event EventHandler<TabSelectionChangedEventArgs>? SelectedTabChanged;

    /// <summary>
    /// Bindable property for SelectedTabKey.
    /// </summary>
    public static readonly BindableProperty SelectedTabKeyProperty = BindableProperty.Create(
        nameof(SelectedTabKey),
        typeof(string),
        typeof(CustomTabsHostPage),
        default(string),
        BindingMode.TwoWay,
        propertyChanged: OnSelectedTabKeyChanged);

    /// <summary>
    /// Bindable property for SelectedIndex.
    /// </summary>
    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex),
        typeof(int),
        typeof(CustomTabsHostPage),
        -1,
        BindingMode.TwoWay,
        propertyChanged: OnSelectedIndexChanged);

    private CustomTabsViewModel? ViewModel => BindingContext as CustomTabsViewModel;

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            Debug.WriteLine("[CustomTabs] CustomTabsHostPage OnAppearing.");
            SubscribeViewModel();
            ApplyTabBarBindingContext(ViewModel);
            ApplyTabBarPlacement();
            SubscribeLocalization();
            UpdateSafeAreaPadding();
        }
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, "CustomTabsHostPage.OnAppearing");
        }
    }

    /// <inheritdoc />
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        try
        {
            Debug.WriteLine("[CustomTabs] CustomTabsHostPage OnBindingContextChanged.");
            UnsubscribeViewModel();
            SubscribeViewModel();
            SyncSelectionFromViewModel();
            ApplyTabBarBindingContext(ViewModel);
            ApplyTabBarPlacement();
        }
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, "CustomTabsHostPage.OnBindingContextChanged");
        }
    }

    /// <inheritdoc />
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        try
        {
            Loaded -= OnHostLoaded;
            UnsubscribeLocalization();
            UnsubscribeViewModel();
#if WINDOWS
            DetachKeyboardHandlers();
#endif
        }
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, "CustomTabsHostPage.OnDisappearing");
        }
    }

    /// <inheritdoc />
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if WINDOWS
        DetachKeyboardHandlers();
        AttachKeyboardHandlers();
#endif
    }

    /// <inheritdoc />
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        try
        {
            if (_layoutLogged)
            {
                return;
            }

            _layoutLogged = true;
            var tabBar = GetTabBar();
            var tabBarHeight = tabBar?.Height ?? -1;
            var tabBarVisible = tabBar?.IsVisible ?? false;
            Debug.WriteLine($"[CustomTabs] Host size={width}x{height}, tab bar height={tabBarHeight}, visible={tabBarVisible}.");
        }
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, "CustomTabsHostPage.OnSizeAllocated");
        }
    }

    /// <inheritdoc />
    protected override bool OnBackButtonPressed()
    {
        try
        {
            if (DeviceInfo.Platform == DevicePlatform.Android && ViewModel?.SelectedTab?.NavigationPage != null)
            {
                var nav = ViewModel.SelectedTab.NavigationPage;
                if (nav.Navigation.NavigationStack.Count > 1)
                {
                    SafeFireAndForget(nav.PopAsync(), "CustomTabsHostPage.OnBackButtonPressed.PopAsync");
                    return true;
                }
            }

            return base.OnBackButtonPressed();
        }
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, "CustomTabsHostPage.OnBackButtonPressed");
            return base.OnBackButtonPressed();
        }
    }

    private static void OnSelectedTabKeyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var page = (CustomTabsHostPage)bindable;
        if (page._suppressSelectionSync)
        {
            return;
        }

        var key = newValue as string;
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        page.ViewModel?.SelectTabByKey(key);
    }

    private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var page = (CustomTabsHostPage)bindable;
        if (page._suppressSelectionSync)
        {
            return;
        }

        if (newValue is int index)
        {
            page.ViewModel?.SelectTabByIndex(index);
        }
    }

    private void SubscribeViewModel()
    {
        var viewModel = ViewModel;
        if (viewModel == null)
        {
            return;
        }

        if (ReferenceEquals(_subscribedViewModel, viewModel))
        {
            return;
        }

        UnsubscribeViewModel();

        viewModel.Options.PropertyChanged += OnOptionsPropertyChanged;
        viewModel.TabSelectionRequested += OnTabSelectionRequested;
        viewModel.SelectedTabChanged += OnSelectedTabChanged;
        viewModel.CanSelectTab = _canSelectTab;
        _subscribedViewModel = viewModel;
    }

    private void UnsubscribeViewModel()
    {
        var viewModel = _subscribedViewModel;
        if (viewModel == null)
        {
            return;
        }

        viewModel.Options.PropertyChanged -= OnOptionsPropertyChanged;
        viewModel.TabSelectionRequested -= OnTabSelectionRequested;
        viewModel.SelectedTabChanged -= OnSelectedTabChanged;
        _subscribedViewModel = null;
    }

    private void SyncSelectionFromViewModel()
    {
        var viewModel = ViewModel;
        if (viewModel == null)
        {
            return;
        }

        _suppressSelectionSync = true;
        SelectedTabKey = viewModel.SelectedTab?.Key;
        SelectedIndex = viewModel.SelectedIndex;
        _suppressSelectionSync = false;
    }

    private void SubscribeLocalization()
    {
        if (_localizationService == null || _localizationSubscribed)
        {
            return;
        }

        _localizationService.LanguageChanged += OnLanguageChanged;
        _localizationSubscribed = true;
        ViewModel?.RefreshTitles();
    }

    private void UnsubscribeLocalization()
    {
        if (_localizationService == null || !_localizationSubscribed)
        {
            return;
        }

        _localizationService.LanguageChanged -= OnLanguageChanged;
        _localizationSubscribed = false;
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        ViewModel?.RefreshTitles();
    }

    private void OnOptionsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CustomTabsOptions.RespectSafeArea)
            || e.PropertyName == nameof(CustomTabsOptions.VisualStyle))
        {
            ApplyTabBarPlacement();
            UpdateSafeAreaPadding();
        }
#if WINDOWS
        if (e.PropertyName == nameof(CustomTabsOptions.EnableKeyboardNavigation))
        {
            DetachKeyboardHandlers();
            AttachKeyboardHandlers();
        }
#endif
    }

    private void OnTabSelectionRequested(object? sender, TabSelectionRequestedEventArgs e)
    {
        TabSelectionRequested?.Invoke(this, e);
    }

    private void OnSelectedTabChanged(object? sender, TabSelectionChangedEventArgs e)
    {
        SyncSelectionFromViewModel();
        SelectedTabChanged?.Invoke(this, e);
    }

    private void OnHostLoaded(object? sender, EventArgs e)
    {
        try
        {
            EnsureTabBarPresent();
            ApplyTabBarBindingContext(ViewModel);
            ApplyTabBarPlacement();
            UpdateSafeAreaPadding();

            var tabBar = GetTabBar();
            Debug.WriteLine($"[CustomTabs] CustomTabsHostPage loaded. TabBar found={tabBar != null}, visible={tabBar?.IsVisible}, tabs={ViewModel?.Tabs.Count ?? 0}.");
        }
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, "CustomTabsHostPage.OnHostLoaded");
        }
    }

    private void EnsureTabBarPresent()
    {
        var viewModel = ViewModel;
        if (viewModel == null)
        {
            return;
        }

        var tabBar = GetTabBar();
        if (tabBar != null)
        {
            return;
        }

        var tabBarHost = GetTabBarHost();
        var contentHost = GetContentHost();
        if (tabBarHost == null || contentHost == null)
        {
            Debug.WriteLine("[CustomTabs] Rebuilding host layout because named elements were missing.");

            var rebuiltRoot = new Grid();
            rebuiltRoot.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            rebuiltRoot.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            var rebuiltContent = new ContentView();
            rebuiltContent.SetBinding(ContentView.ContentProperty, new Binding(nameof(CustomTabsViewModel.SelectedContent)));
            rebuiltContent.SetBinding(Microsoft.Maui.Controls.VisualElement.BackgroundColorProperty, new Binding("Options.EffectiveContentBackgroundColor"));
            Grid.SetRow(rebuiltContent, 0);

            var rebuiltTabHost = new Grid();
            Grid.SetRow(rebuiltTabHost, 1);
            rebuiltTabHost.SetBinding(Microsoft.Maui.Controls.VisualElement.BackgroundColorProperty, new Binding("Options.BackgroundColor"));

            var rebuiltTabBar = new CustomTabBarView
            {
                ViewModel = viewModel,
                BindingContext = viewModel
            };
            rebuiltTabBar.SetBinding(View.MarginProperty, new Binding("Options.TabBarOuterMargin"));
            rebuiltTabHost.Children.Add(rebuiltTabBar);

            rebuiltRoot.Children.Add(rebuiltContent);
            rebuiltRoot.Children.Add(rebuiltTabHost);
            Content = rebuiltRoot;
            return;
        }

        Debug.WriteLine("[CustomTabs] Creating fallback tab bar in existing host.");
        var fallbackTabBar = new CustomTabBarView
        {
            ViewModel = viewModel,
            BindingContext = viewModel
        };
        fallbackTabBar.SetBinding(View.MarginProperty, new Binding("Options.TabBarOuterMargin"));
        tabBarHost.SetBinding(Microsoft.Maui.Controls.VisualElement.BackgroundColorProperty, new Binding("Options.BackgroundColor"));
        tabBarHost.Children.Clear();
        tabBarHost.Children.Add(fallbackTabBar);
    }

    private void UpdateSafeAreaPadding()
    {
        var viewModel = ViewModel;
        if (viewModel == null)
        {
            return;
        }

        var tabBarHost = GetTabBarHost();
        var contentHost = GetContentHost();
        var tabBar = GetTabBar();
        if (tabBarHost == null || contentHost == null)
        {
            return;
        }

        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            var config = this.On<Microsoft.Maui.Controls.PlatformConfiguration.iOS>();
            Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(config, false);
        }

        if (!viewModel.Options.RespectSafeArea)
        {
            tabBarHost.Padding = Thickness.Zero;
            contentHost.Padding = Thickness.Zero;
            tabBarHost.Margin = Thickness.Zero;
            if (tabBar != null)
            {
                tabBar.SafeAreaInsets = Thickness.Zero;
            }
            return;
        }

        var insets = GetPlatformSafeAreaPadding();
        var isTop = viewModel.Options.VisualStyle == TabVisualStyle.TopUnderline;

        tabBarHost.Padding = new Thickness(insets.Left, 0, insets.Right, 0);
        contentHost.Padding = isTop
            ? new Thickness(insets.Left, 0, insets.Right, insets.Bottom)
            : new Thickness(insets.Left, insets.Top, insets.Right, 0);

        if (tabBar != null)
        {
            tabBar.SafeAreaInsets = isTop
                ? new Thickness(0, insets.Top, 0, 0)
                : new Thickness(0, 0, 0, insets.Bottom);
        }

        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            var offset = isTop ? -insets.Top : -insets.Bottom;
            tabBarHost.Margin = offset == 0
                ? Thickness.Zero
                : new Thickness(0, isTop ? offset : 0, 0, !isTop ? offset : 0);
        }
        else
        {
            tabBarHost.Margin = Thickness.Zero;
        }
    }

    private void ApplyTabBarBindingContext(CustomTabsViewModel? viewModel)
    {
        if (viewModel == null)
        {
            return;
        }

        var tabBar = GetTabBar();
        if (tabBar == null)
        {
            return;
        }

        if (!ReferenceEquals(tabBar.ViewModel, viewModel))
        {
            tabBar.ViewModel = viewModel;
            Debug.WriteLine("[CustomTabs] CustomTabsHostPage applied tab bar view model.");
        }
    }

    private void ApplyTabBarPlacement()
    {
        var rootGrid = GetRootGrid();
        var tabBarHost = GetTabBarHost();
        var contentHost = GetContentHost();
        if (rootGrid == null || tabBarHost == null || contentHost == null || rootGrid.RowDefinitions.Count < 2)
        {
            return;
        }

        var top = ViewModel?.Options.VisualStyle == TabVisualStyle.TopUnderline;

        if (top)
        {
            rootGrid.RowDefinitions[0].Height = GridLength.Auto;
            rootGrid.RowDefinitions[1].Height = GridLength.Star;
            Grid.SetRow(tabBarHost, 0);
            Grid.SetRow(contentHost, 1);
            return;
        }

        rootGrid.RowDefinitions[0].Height = GridLength.Star;
        rootGrid.RowDefinitions[1].Height = GridLength.Auto;
        Grid.SetRow(contentHost, 0);
        Grid.SetRow(tabBarHost, 1);
    }

    private Grid? GetRootGrid()
        => RootGrid ?? (FindByName("RootGrid") as Grid);

    private Grid? GetTabBarHost()
        => TabBarHost ?? (FindByName("TabBarHost") as Grid);

    private ContentView? GetContentHost()
        => ContentHost ?? (FindByName("ContentHost") as ContentView);

    private CustomTabBarView? GetTabBar()
        => TabBar ?? (FindByName("TabBar") as CustomTabBarView);

    private Thickness GetPlatformSafeAreaPadding()
    {
#if IOS
        try
        {
            var keyWindow = GetKeyWindow();
            if (keyWindow != null)
            {
                var insets = keyWindow.SafeAreaInsets;
                return new Thickness(insets.Left, insets.Top, insets.Right, insets.Bottom);
            }
        }
        catch
        {
            // ignore
        }
#endif

#if ANDROID
        try
        {
            var activity = Platform.CurrentActivity;
            var decorView = activity?.Window?.DecorView;
            if (decorView == null)
            {
                return Thickness.Zero;
            }

            var insets = ViewCompat.GetRootWindowInsets(decorView);
            if (insets == null)
            {
                return Thickness.Zero;
            }

            var systemInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.DisplayCutout());
            var density = DeviceDisplay.MainDisplayInfo.Density;
            if (density <= 0)
            {
                return Thickness.Zero;
            }

            return new Thickness(
                systemInsets.Left / density,
                systemInsets.Top / density,
                systemInsets.Right / density,
                systemInsets.Bottom / density);
        }
        catch
        {
            return Thickness.Zero;
        }
#else
        return Thickness.Zero;
#endif
    }

#if IOS
    private static UIWindow? GetKeyWindow()
    {
        var keyWindow = UIApplication.SharedApplication.KeyWindow;
        if (keyWindow != null)
        {
            return keyWindow;
        }

        foreach (var scene in UIApplication.SharedApplication.ConnectedScenes)
        {
            if (scene is UIWindowScene windowScene)
            {
                foreach (var window in windowScene.Windows)
                {
                    if (window.IsKeyWindow)
                    {
                        return window;
                    }
                }
            }
        }

        foreach (var window in UIApplication.SharedApplication.Windows)
        {
            return window;
        }

        return null;
    }
#endif

#if WINDOWS
    private void AttachKeyboardHandlers()
    {
        if (_keyboardAttached)
        {
            return;
        }

        var viewModel = ViewModel;
        if (viewModel == null || !viewModel.Options.EnableKeyboardNavigation)
        {
            return;
        }

        if (Handler?.PlatformView is FrameworkElement element)
        {
            element.KeyDown += OnWindowsKeyDown;
            _keyboardAttached = true;
        }
    }

    private void DetachKeyboardHandlers()
    {
        if (!_keyboardAttached)
        {
            return;
        }

        if (Handler?.PlatformView is FrameworkElement element)
        {
            element.KeyDown -= OnWindowsKeyDown;
        }

        _keyboardAttached = false;
    }

    private void OnWindowsKeyDown(object sender, KeyRoutedEventArgs e)
    {
        var viewModel = ViewModel;
        if (viewModel == null || !viewModel.Options.EnableKeyboardNavigation)
        {
            return;
        }

        switch (e.Key)
        {
            case VirtualKey.Left:
            case VirtualKey.GamepadDPadLeft:
                SelectRelativeTab(-1);
                e.Handled = true;
                break;
            case VirtualKey.Right:
            case VirtualKey.GamepadDPadRight:
                SelectRelativeTab(1);
                e.Handled = true;
                break;
            case VirtualKey.Home:
                SelectFirstTab();
                e.Handled = true;
                break;
            case VirtualKey.End:
                SelectLastTab();
                e.Handled = true;
                break;
            case VirtualKey.Enter:
            case VirtualKey.Space:
                SafeFireAndForget(TriggerReselectAsync(), "CustomTabsHostPage.OnWindowsKeyDown.TriggerReselectAsync");
                e.Handled = true;
                break;
        }
    }
#endif

    private void SelectRelativeTab(int delta)
    {
        var viewModel = ViewModel;
        if (viewModel == null || viewModel.Tabs.Count == 0)
        {
            return;
        }

        var count = viewModel.Tabs.Count;
        var startIndex = viewModel.SelectedIndex;
        var index = startIndex < 0 ? 0 : startIndex;

        for (var i = 0; i < count; i++)
        {
            index = (index + delta + count) % count;
            var tab = viewModel.Tabs[index];
            if (tab.IsVisible && tab.IsEnabled)
            {
                viewModel.SelectTabByIndex(index);
                return;
            }
        }
    }

    private void SelectFirstTab()
    {
        var viewModel = ViewModel;
        if (viewModel == null)
        {
            return;
        }

        for (var i = 0; i < viewModel.Tabs.Count; i++)
        {
            var tab = viewModel.Tabs[i];
            if (tab.IsVisible && tab.IsEnabled)
            {
                viewModel.SelectTabByIndex(i);
                return;
            }
        }
    }

    private void SelectLastTab()
    {
        var viewModel = ViewModel;
        if (viewModel == null)
        {
            return;
        }

        for (var i = viewModel.Tabs.Count - 1; i >= 0; i--)
        {
            var tab = viewModel.Tabs[i];
            if (tab.IsVisible && tab.IsEnabled)
            {
                viewModel.SelectTabByIndex(i);
                return;
            }
        }
    }

    private async Task TriggerReselectAsync()
    {
        try
        {
            var viewModel = ViewModel;
            var tab = viewModel?.SelectedTab;
            if (viewModel == null || tab == null)
            {
                return;
            }

            var result = viewModel.TrySelectTab(tab, TabSelectionOrigin.User, allowReselect: true);
            if (result == TabSelectionResult.Reselected)
            {
                await viewModel.HandleReselectAsync(tab);
            }
        }
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, "CustomTabsHostPage.TriggerReselectAsync");
        }
    }

    private static void SafeFireAndForget(Task task, string context)
    {
        if (task.IsCompletedSuccessfully)
        {
            return;
        }

        _ = ObserveTaskAsync(task, context);
    }

    private static async Task ObserveTaskAsync(Task task, string context)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, context);
        }
    }
}
