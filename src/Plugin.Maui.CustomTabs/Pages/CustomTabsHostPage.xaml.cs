using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Devices;
using Microsoft.Maui;
using Plugin.Maui.CustomTabs.Controls;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Services;
using Plugin.Maui.CustomTabs.ViewModels;
using System.Diagnostics;
using System.Linq;
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
    private Grid? _tabBarHost;
    private CustomTabBarView? _tabBarView;
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
        BindingContext = viewModel;
        _localizationService = localizationService;

        EnsureLayout(viewModel);
        _tabBarHost ??= ResolveTabBarHost();
        ApplyTabBarBindingContext(viewModel);
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
        Debug.WriteLine("[CustomTabs] CustomTabsHostPage OnAppearing.");
        SubscribeViewModel();
        EnsureLayout(ViewModel);
        ApplyTabBarBindingContext(ViewModel);
        SubscribeLocalization();
        UpdateSafeAreaPadding();
    }

    /// <inheritdoc />
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        Debug.WriteLine("[CustomTabs] CustomTabsHostPage OnBindingContextChanged.");
        UnsubscribeViewModel();
        SubscribeViewModel();
        SyncSelectionFromViewModel();
        EnsureLayout(ViewModel);
        ApplyTabBarBindingContext(ViewModel);
    }

    /// <inheritdoc />
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        UnsubscribeLocalization();
        UnsubscribeViewModel();
#if WINDOWS
        DetachKeyboardHandlers();
#endif
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

        if (_layoutLogged || _tabBarView == null)
        {
            return;
        }

        _layoutLogged = true;
        Debug.WriteLine($"[CustomTabs] Host size={width}x{height}, tab bar height={_tabBarView.Height}, visible={_tabBarView.IsVisible}.");
    }

    /// <inheritdoc />
    protected override bool OnBackButtonPressed()
    {
        if (DeviceInfo.Platform == DevicePlatform.Android && ViewModel?.SelectedTab?.NavigationPage != null)
        {
            var nav = ViewModel.SelectedTab.NavigationPage;
            if (nav.Navigation.NavigationStack.Count > 1)
            {
                _ = nav.PopAsync();
                return true;
            }
        }

        return base.OnBackButtonPressed();
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
        if (e.PropertyName == nameof(CustomTabsOptions.RespectSafeArea))
        {
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

    private void UpdateSafeAreaPadding()
    {
        var viewModel = ViewModel;
        if (viewModel == null)
        {
            return;
        }

        if (_tabBarHost == null)
        {
            _tabBarHost = ResolveTabBarHost();
            if (_tabBarHost == null)
            {
                return;
            }
        }

        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            this.On<iOS>().SetUseSafeArea(viewModel.Options.RespectSafeArea);
            _tabBarHost.Padding = Thickness.Zero;
            return;
        }

        if (!viewModel.Options.RespectSafeArea)
        {
            _tabBarHost.Padding = Thickness.Zero;
            return;
        }

        _tabBarHost.Padding = GetPlatformSafeAreaPadding();
    }

    private Grid? ResolveTabBarHost()
    {
        if (Content is not Grid root || root.Children.Count < 2)
        {
            return null;
        }

        return root.Children[1] as Grid;
    }

    private void ApplyTabBarBindingContext(CustomTabsViewModel? viewModel)
    {
        if (viewModel == null)
        {
            return;
        }

        _tabBarHost ??= ResolveTabBarHost();
        _tabBarView ??= ResolveTabBarView();
        if (_tabBarView == null)
        {
            EnsureLayout(viewModel);
            _tabBarView ??= ResolveTabBarView();
            if (_tabBarView == null)
            {
                Debug.WriteLine("[CustomTabs] CustomTabsHostPage tab bar view not found.");
                return;
            }
        }

        if (!ReferenceEquals(_tabBarView.ViewModel, viewModel))
        {
            _tabBarView.ViewModel = viewModel;
            Debug.WriteLine("[CustomTabs] CustomTabsHostPage applied tab bar view model.");
        }
    }

    private void EnsureLayout(CustomTabsViewModel? viewModel)
    {
        if (Content is Grid existingGrid && FindTabBarView(existingGrid) != null)
        {
            return;
        }

        var rootGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto }
            }
        };

        var contentHost = new ContentView();
        contentHost.SetBinding(ContentView.ContentProperty, new Binding(nameof(CustomTabsViewModel.SelectedContent)));
        rootGrid.Children.Add(contentHost);
        Grid.SetRow(contentHost, 0);

        var tabHost = new Grid();
        var tabBar = new CustomTabBarView();
        tabBar.SetBinding(CustomTabBarView.ViewModelProperty, new Binding("."));
        tabBar.SetBinding(Microsoft.Maui.Controls.View.MarginProperty, new Binding("Options.TabBarOuterMargin"));
        tabHost.Children.Add(tabBar);
        rootGrid.Children.Add(tabHost);
        Grid.SetRow(tabHost, 1);

        Content = rootGrid;
        _tabBarHost = tabHost;
        _tabBarView = tabBar;

        if (viewModel != null)
        {
            SetBinding(BackgroundColorProperty, new Binding("Options.BackgroundColor"));
        }

        Debug.WriteLine("[CustomTabs] CustomTabsHostPage fallback layout created.");
    }

    private CustomTabBarView? ResolveTabBarView()
    {
        if (_tabBarView != null)
        {
            return _tabBarView;
        }

        if (_tabBarHost != null)
        {
            _tabBarView = _tabBarHost.Children.OfType<CustomTabBarView>().FirstOrDefault();
            if (_tabBarView != null)
            {
                return _tabBarView;
            }
        }

        _tabBarView = FindTabBarView(Content);
        return _tabBarView;
    }

    private static CustomTabBarView? FindTabBarView(IView? view)
    {
        if (view == null)
        {
            return null;
        }

        if (view is CustomTabBarView tabBar)
        {
            return tabBar;
        }

        if (view is ContentView contentView)
        {
            return FindTabBarView(contentView.Content);
        }

        if (view is Microsoft.Maui.Controls.ScrollView scrollView)
        {
            return FindTabBarView(scrollView.Content);
        }

        if (view is Border border)
        {
            return FindTabBarView(border.Content);
        }

        if (view is Layout layout)
        {
            foreach (var child in layout.Children)
            {
                var found = FindTabBarView(child);
                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }

    private Thickness GetPlatformSafeAreaPadding()
    {
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
                0,
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
                _ = TriggerReselectAsync();
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
}
