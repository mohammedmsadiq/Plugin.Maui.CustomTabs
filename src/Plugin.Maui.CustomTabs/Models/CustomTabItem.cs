using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Diagnostics;

namespace Plugin.Maui.CustomTabs.Models;

/// <summary>
/// Describes a single tab with a lazily created navigation stack.
/// </summary>
public sealed class CustomTabItem : INotifyPropertyChanged
{
    private string? _title;
    private Func<string>? _titleProvider;
    private Func<ImageSource>? _iconProvider;
    private ImageSource? _iconSource;
    private CustomTabBadge? _badge;
    private string _iconGlyph;
    private bool _isEnabled = true;
    private bool _isVisible = true;
    private string? _automationId;
    private string? _automationName;
    private string? _automationDescription;
    private Color? _selectedTextColor;
    private Color? _unselectedTextColor;
    private Color? _selectedIconColor;
    private Color? _unselectedIconColor;
    private Color? _indicatorColor;
    private Func<Page, NavigationPage>? _navigationPageFactory;
    private bool? _navigationBarVisible;
    private Color? _navigationBarBackgroundColor;
    private Color? _navigationBarTextColor;
    private ICommand? _reselectCommand;
    private object? _reselectCommandParameter;
    private bool _navigationHandlersAttached;
    private readonly Dictionary<Page, View> _contentCache = new();

    internal NavigationPage? NavigationPage { get; private set; }

    /// <summary>
    /// Raised when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Creates a new tab item.
    /// </summary>
    /// <param name="key">Unique key for the tab.</param>
    /// <param name="iconGlyph">Glyph to render when using a font icon.</param>
    /// <param name="createRootPage">Factory that creates the root page.</param>
    public CustomTabItem(string key, string iconGlyph, Func<Page> createRootPage)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        _iconGlyph = iconGlyph ?? string.Empty;
        CreateRootPage = createRootPage ?? throw new ArgumentNullException(nameof(createRootPage));
    }

    /// <summary>
    /// Unique key used to select the tab.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Font glyph used for the icon.
    /// </summary>
    public string IconGlyph
    {
        get => _iconGlyph;
        set
        {
            if (SetProperty(ref _iconGlyph, value))
            {
                OnPropertyChanged(nameof(HasGlyphIcon));
            }
        }
    }

    /// <summary>
    /// Factory used to create the root page for the tab.
    /// </summary>
    public Func<Page> CreateRootPage { get; }

    /// <summary>
    /// Optional factory to create the navigation page for this tab.
    /// </summary>
    public Func<Page, NavigationPage>? NavigationPageFactory
    {
        get => _navigationPageFactory;
        set => SetProperty(ref _navigationPageFactory, value);
    }

    /// <summary>
    /// Override for navigation bar visibility on this tab.
    /// </summary>
    public bool? NavigationBarVisible
    {
        get => _navigationBarVisible;
        set => SetProperty(ref _navigationBarVisible, value);
    }

    /// <summary>
    /// Override for navigation bar background color on this tab.
    /// </summary>
    public Color? NavigationBarBackgroundColor
    {
        get => _navigationBarBackgroundColor;
        set => SetProperty(ref _navigationBarBackgroundColor, value);
    }

    /// <summary>
    /// Override for navigation bar text color on this tab.
    /// </summary>
    public Color? NavigationBarTextColor
    {
        get => _navigationBarTextColor;
        set => SetProperty(ref _navigationBarTextColor, value);
    }

    /// <summary>
    /// Static title string (optional).
    /// </summary>
    public string? Title
    {
        get => _title;
        set
        {
            if (SetProperty(ref _title, value))
            {
                OnPropertyChanged(nameof(TitleText));
            }
        }
    }

    /// <summary>
    /// Dynamic title provider (optional).
    /// </summary>
    public Func<string>? TitleProvider
    {
        get => _titleProvider;
        set
        {
            if (SetProperty(ref _titleProvider, value))
            {
                OnPropertyChanged(nameof(TitleText));
            }
        }
    }

    /// <summary>
    /// Icon provider for image-based icons (optional).
    /// </summary>
    public Func<ImageSource>? IconProvider
    {
        get => _iconProvider;
        set
        {
            if (SetProperty(ref _iconProvider, value))
            {
                NotifyIconChanged();
            }
        }
    }

    /// <summary>
    /// Static icon source for image-based icons (optional).
    /// </summary>
    public ImageSource? IconSource
    {
        get => _iconSource;
        set
        {
            if (SetProperty(ref _iconSource, value))
            {
                NotifyIconChanged();
            }
        }
    }

    /// <summary>
    /// Optional badge for the tab.
    /// </summary>
    public CustomTabBadge? Badge
    {
        get => _badge;
        set => SetProperty(ref _badge, value);
    }

    /// <summary>
    /// True when the tab is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    /// <summary>
    /// True when the tab is visible.
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    /// <summary>
    /// Optional automation id for UI testing.
    /// </summary>
    public string? AutomationId
    {
        get => _automationId;
        set => SetProperty(ref _automationId, value);
    }

    /// <summary>
    /// Optional automation name for accessibility.
    /// </summary>
    public string? AutomationName
    {
        get => _automationName;
        set => SetProperty(ref _automationName, value);
    }

    /// <summary>
    /// Optional automation description for accessibility.
    /// </summary>
    public string? AutomationDescription
    {
        get => _automationDescription;
        set => SetProperty(ref _automationDescription, value);
    }

    /// <summary>
    /// Optional per-tab selected text color.
    /// </summary>
    public Color? SelectedTextColor
    {
        get => _selectedTextColor;
        set => SetProperty(ref _selectedTextColor, value);
    }

    /// <summary>
    /// Optional per-tab unselected text color.
    /// </summary>
    public Color? UnselectedTextColor
    {
        get => _unselectedTextColor;
        set => SetProperty(ref _unselectedTextColor, value);
    }

    /// <summary>
    /// Optional per-tab selected icon color.
    /// </summary>
    public Color? SelectedIconColor
    {
        get => _selectedIconColor;
        set => SetProperty(ref _selectedIconColor, value);
    }

    /// <summary>
    /// Optional per-tab unselected icon color.
    /// </summary>
    public Color? UnselectedIconColor
    {
        get => _unselectedIconColor;
        set => SetProperty(ref _unselectedIconColor, value);
    }

    /// <summary>
    /// Optional per-tab indicator color.
    /// </summary>
    public Color? IndicatorColor
    {
        get => _indicatorColor;
        set => SetProperty(ref _indicatorColor, value);
    }

    /// <summary>
    /// Command executed when the tab is reselected.
    /// </summary>
    public ICommand? ReselectCommand
    {
        get => _reselectCommand;
        set => SetProperty(ref _reselectCommand, value);
    }

    /// <summary>
    /// Parameter passed to the reselect command.
    /// </summary>
    public object? ReselectCommandParameter
    {
        get => _reselectCommandParameter;
        set => SetProperty(ref _reselectCommandParameter, value);
    }

    /// <summary>
    /// Computed title used by the UI.
    /// </summary>
    public string TitleText => TitleProvider?.Invoke() ?? Title ?? string.Empty;

    /// <summary>
    /// Returns true when the item has an image icon.
    /// </summary>
    public bool HasImageIcon => IconSource != null || IconProvider != null;

    /// <summary>
    /// Returns true when the item has a glyph icon.
    /// </summary>
    public bool HasGlyphIcon => !string.IsNullOrWhiteSpace(IconGlyph) && !HasImageIcon;

    /// <summary>
    /// Gets the image icon produced by <see cref="IconSource"/> or <see cref="IconProvider"/>.
    /// </summary>
    public ImageSource? IconImage => IconSource ?? IconProvider?.Invoke();

    /// <summary>
    /// Current page displayed in the navigation stack.
    /// </summary>
    public Page? CurrentPage => NavigationPage?.CurrentPage;

    /// <summary>
    /// Current content view displayed in the navigation stack.
    /// </summary>
    public View? CurrentContent
    {
        get
        {
            var page = NavigationPage?.CurrentPage;
            if (page == null)
            {
                return null;
            }

            return ResolvePageContent(page);
        }
    }

    /// <summary>
    /// Forces the UI to re-evaluate the title.
    /// </summary>
    public void NotifyTitleChanged() => OnPropertyChanged(nameof(TitleText));

    /// <summary>
    /// Forces the UI to refresh icon bindings.
    /// </summary>
    public void NotifyIconChanged()
    {
        OnPropertyChanged(nameof(HasImageIcon));
        OnPropertyChanged(nameof(HasGlyphIcon));
        OnPropertyChanged(nameof(IconImage));
    }

    /// <summary>
    /// Lazily creates or returns the navigation page for the tab.
    /// </summary>
    public NavigationPage GetOrCreateNavigationPage(CustomTabsOptions? options = null)
    {
        if (NavigationPage != null)
        {
            return NavigationPage;
        }

        var rootPage = CreateRootPage();
        var factory = NavigationPageFactory ?? options?.NavigationPageFactory;
        NavigationPage = factory?.Invoke(rootPage) ?? new NavigationPage(rootPage);
        AttachNavigationHandlers(NavigationPage);
        Debug.WriteLine($"[CustomTabs] Created NavigationPage for tab '{Key}' using {(factory == null ? "default" : "custom")} factory.");

        ApplyNavigationSettings(rootPage, NavigationPage, options);
        OnPropertyChanged(nameof(CurrentPage));
        OnPropertyChanged(nameof(CurrentContent));

        return NavigationPage;
    }

    private void AttachNavigationHandlers(NavigationPage navigationPage)
    {
        if (_navigationHandlersAttached)
        {
            return;
        }

        navigationPage.Pushed += OnNavigationChanged;
        navigationPage.Popped += OnNavigationChanged;
        navigationPage.PoppedToRoot += OnNavigationChanged;
        _navigationHandlersAttached = true;
    }

    private void OnNavigationChanged(object? sender, NavigationEventArgs e)
    {
        OnPropertyChanged(nameof(CurrentPage));
        OnPropertyChanged(nameof(CurrentContent));
    }

    private View? ResolvePageContent(Page page)
    {
        if (_contentCache.TryGetValue(page, out var cached))
        {
            cached.BindingContext = page.BindingContext;
            return cached;
        }

        if (page is not ContentPage contentPage)
        {
            return null;
        }

        var content = contentPage.Content;
        if (content == null)
        {
            return null;
        }

        contentPage.Content = null;
        content.BindingContext = page.BindingContext;
        _contentCache[page] = content;
        return content;
    }

    private void ApplyNavigationSettings(Page rootPage, NavigationPage navigationPage, CustomTabsOptions? options)
    {
        var navBarVisible = NavigationBarVisible ?? options?.NavigationBarVisible;
        if (navBarVisible.HasValue)
        {
            NavigationPage.SetHasNavigationBar(rootPage, navBarVisible.Value);
        }

        var backgroundColor = NavigationBarBackgroundColor ?? options?.NavigationBarBackgroundColor;
        if (backgroundColor != null)
        {
            navigationPage.BarBackgroundColor = backgroundColor;
        }

        var textColor = NavigationBarTextColor ?? options?.NavigationBarTextColor;
        if (textColor != null)
        {
            navigationPage.BarTextColor = textColor;
        }
    }

    private bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(backingStore, value))
        {
            return false;
        }

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
