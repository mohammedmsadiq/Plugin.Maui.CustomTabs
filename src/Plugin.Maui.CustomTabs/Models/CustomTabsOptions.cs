using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;

namespace Plugin.Maui.CustomTabs.Models;

/// <summary>
/// Configuration options for the custom tab bar.
/// </summary>
public sealed class CustomTabsOptions : INotifyPropertyChanged
{
    private bool _showText = true;
    private double _tabBarHeight = 76;
    private Color _backgroundColor = Color.FromArgb("#071A3A");
    private Color _accentColor = Color.FromArgb("#D4AF37");
    private Color _selectedTextColor = Colors.White;
    private Color _unselectedTextColor = Colors.LightGray;
    private Color _selectedIconColor = Color.FromArgb("#D4AF37");
    private Color _unselectedIconColor = Colors.LightGray;
    private bool _enableHaptics;
    private bool _enableAnimations = true;
    private TimeSpan _animationDuration = TimeSpan.FromMilliseconds(160);
    private string? _fontFamily;
    private bool _respectSafeArea = true;
    private double _iconSize = 26;
    private double _textSize = 12;
    private Thickness _tabBarPadding = new Thickness(0, 10, 0, 16);
    private Thickness _tabBarOuterMargin = Thickness.Zero;
    private bool _showUnderline = true;
    private double _underlineHeight = 2;
    private Thickness _underlineMargin = new Thickness(0, 6, 0, 0);
    private double _badgeOffsetX = 6;
    private double _badgeOffsetY = -4;

    /// <summary>
    /// Raised when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Shows or hides tab text.
    /// </summary>
    public bool ShowText
    {
        get => _showText;
        set => SetProperty(ref _showText, value);
    }

    /// <summary>
    /// Height of the tab bar.
    /// </summary>
    public double TabBarHeight
    {
        get => _tabBarHeight;
        set => SetProperty(ref _tabBarHeight, value);
    }

    /// <summary>
    /// Background color for the tab bar.
    /// </summary>
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set => SetProperty(ref _backgroundColor, value);
    }

    /// <summary>
    /// Accent color for the underline and selected elements.
    /// </summary>
    public Color AccentColor
    {
        get => _accentColor;
        set => SetProperty(ref _accentColor, value);
    }

    /// <summary>
    /// Text color for selected tabs.
    /// </summary>
    public Color SelectedTextColor
    {
        get => _selectedTextColor;
        set => SetProperty(ref _selectedTextColor, value);
    }

    /// <summary>
    /// Text color for unselected tabs.
    /// </summary>
    public Color UnselectedTextColor
    {
        get => _unselectedTextColor;
        set => SetProperty(ref _unselectedTextColor, value);
    }

    /// <summary>
    /// Icon color for selected tabs.
    /// </summary>
    public Color SelectedIconColor
    {
        get => _selectedIconColor;
        set => SetProperty(ref _selectedIconColor, value);
    }

    /// <summary>
    /// Icon color for unselected tabs.
    /// </summary>
    public Color UnselectedIconColor
    {
        get => _unselectedIconColor;
        set => SetProperty(ref _unselectedIconColor, value);
    }

    /// <summary>
    /// Enables haptic feedback on selection.
    /// </summary>
    public bool EnableHaptics
    {
        get => _enableHaptics;
        set => SetProperty(ref _enableHaptics, value);
    }

    /// <summary>
    /// Enables selection animations.
    /// </summary>
    public bool EnableAnimations
    {
        get => _enableAnimations;
        set => SetProperty(ref _enableAnimations, value);
    }

    /// <summary>
    /// Duration of selection animations.
    /// </summary>
    public TimeSpan AnimationDuration
    {
        get => _animationDuration;
        set => SetProperty(ref _animationDuration, value);
    }

    /// <summary>
    /// Optional font family for labels and glyphs.
    /// </summary>
    public string? FontFamily
    {
        get => _fontFamily;
        set => SetProperty(ref _fontFamily, value);
    }

    /// <summary>
    /// Respect safe area insets where available.
    /// </summary>
    public bool RespectSafeArea
    {
        get => _respectSafeArea;
        set => SetProperty(ref _respectSafeArea, value);
    }

    /// <summary>
    /// Size of tab icons.
    /// </summary>
    public double IconSize
    {
        get => _iconSize;
        set => SetProperty(ref _iconSize, value);
    }

    /// <summary>
    /// Size of tab text.
    /// </summary>
    public double TextSize
    {
        get => _textSize;
        set => SetProperty(ref _textSize, value);
    }

    /// <summary>
    /// Padding for the tab bar container.
    /// </summary>
    public Thickness TabBarPadding
    {
        get => _tabBarPadding;
        set => SetProperty(ref _tabBarPadding, value);
    }

    /// <summary>
    /// Outer margin around the tab bar (useful for floating bars).
    /// </summary>
    public Thickness TabBarOuterMargin
    {
        get => _tabBarOuterMargin;
        set => SetProperty(ref _tabBarOuterMargin, value);
    }

    /// <summary>
    /// Shows or hides the underline indicator.
    /// </summary>
    public bool ShowUnderline
    {
        get => _showUnderline;
        set => SetProperty(ref _showUnderline, value);
    }

    /// <summary>
    /// Height of the underline indicator.
    /// </summary>
    public double UnderlineHeight
    {
        get => _underlineHeight;
        set => SetProperty(ref _underlineHeight, value);
    }

    /// <summary>
    /// Margin applied above the underline to control spacing from the text.
    /// </summary>
    public Thickness UnderlineMargin
    {
        get => _underlineMargin;
        set => SetProperty(ref _underlineMargin, value);
    }

    /// <summary>
    /// Horizontal offset for the badge relative to the icon.
    /// </summary>
    public double BadgeOffsetX
    {
        get => _badgeOffsetX;
        set => SetProperty(ref _badgeOffsetX, value);
    }

    /// <summary>
    /// Vertical offset for the badge relative to the icon.
    /// </summary>
    public double BadgeOffsetY
    {
        get => _badgeOffsetY;
        set => SetProperty(ref _badgeOffsetY, value);
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
