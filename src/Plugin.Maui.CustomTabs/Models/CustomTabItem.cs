using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;

namespace Plugin.Maui.CustomTabs.Models;

/// <summary>
/// Describes a single tab with a lazily created navigation stack.
/// </summary>
public sealed class CustomTabItem : INotifyPropertyChanged
{
    private string? _title;
    private Func<string>? _titleProvider;
    private Func<ImageSource>? _iconProvider;
    private CustomTabBadge? _badge;
    private string _iconGlyph;

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
                OnPropertyChanged(nameof(HasImageIcon));
                OnPropertyChanged(nameof(HasGlyphIcon));
                OnPropertyChanged(nameof(IconImage));
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
    /// Computed title used by the UI.
    /// </summary>
    public string TitleText => TitleProvider?.Invoke() ?? Title ?? string.Empty;

    /// <summary>
    /// Returns true when the item has an image icon.
    /// </summary>
    public bool HasImageIcon => IconProvider != null;

    /// <summary>
    /// Returns true when the item has a glyph icon.
    /// </summary>
    public bool HasGlyphIcon => !string.IsNullOrWhiteSpace(IconGlyph) && IconProvider == null;

    /// <summary>
    /// Gets the image icon produced by <see cref="IconProvider"/>.
    /// </summary>
    public ImageSource? IconImage => IconProvider?.Invoke();

    /// <summary>
    /// Forces the UI to re-evaluate the title.
    /// </summary>
    public void NotifyTitleChanged() => OnPropertyChanged(nameof(TitleText));

    /// <summary>
    /// Lazily creates or returns the navigation page for the tab.
    /// </summary>
    public NavigationPage GetOrCreateNavigationPage()
    {
        if (NavigationPage != null)
        {
            return NavigationPage;
        }

        var rootPage = CreateRootPage();
        NavigationPage = new NavigationPage(rootPage);
        return NavigationPage;
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
