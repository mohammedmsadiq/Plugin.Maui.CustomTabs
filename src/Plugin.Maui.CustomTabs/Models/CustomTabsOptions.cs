using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using MauiShadow = Microsoft.Maui.Controls.Shadow;

namespace Plugin.Maui.CustomTabs.Models;

/// <summary>
/// Configuration options for the custom tab bar.
/// </summary>
public sealed class CustomTabsOptions : INotifyPropertyChanged
{
    private bool _showText = true;
    private double _tabBarHeight = 76;
    private Color _backgroundColor = Color.FromArgb("#1F2937");
    private Color _accentColor = Color.FromArgb("#9CA3AF");
    private Color _selectedTextColor = Color.FromArgb("#F9FAFB");
    private Color _unselectedTextColor = Color.FromArgb("#9CA3AF");
    private Color _selectedIconColor = Color.FromArgb("#F9FAFB");
    private Color _unselectedIconColor = Color.FromArgb("#9CA3AF");
    private bool _enableHaptics;
    private bool _enableAnimations = true;
    private TimeSpan _animationDuration = TimeSpan.FromMilliseconds(160);
    private TabSelectionAnimationStyle _iconAnimationStyle = TabSelectionAnimationStyle.Scale;
    private TabSelectionAnimationStyle _indicatorAnimationStyle = TabSelectionAnimationStyle.Scale;
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
    private TabReselectBehavior _reselectBehavior = TabReselectBehavior.None;
    private DataTemplate? _tabItemTemplate;
    private TabLayoutMode _tabLayoutMode = TabLayoutMode.Auto;
    private TabVisualStyle _visualStyle = TabVisualStyle.ClassicBottom;
    private int _scrollableThreshold = 5;
    private double _tabItemWidth = -1;
    private double _tabItemMinWidth;
    private CornerRadius _tabBarCornerRadius;
    private Color _borderColor = Colors.Transparent;
    private double _borderThickness;
    private MauiShadow? _tabBarShadow;
    private double _indicatorWidth = -1;
    private LayoutOptions _indicatorHorizontalOptions = LayoutOptions.Fill;
    private float _indicatorCornerRadius;
    private bool _enableKeyboardNavigation = true;
    private Func<Page, NavigationPage>? _navigationPageFactory;
    private bool? _navigationBarVisible;
    private Color? _navigationBarBackgroundColor;
    private Color? _navigationBarTextColor;

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
    /// Animation style used for icons.
    /// </summary>
    public TabSelectionAnimationStyle IconAnimationStyle
    {
        get => _iconAnimationStyle;
        set => SetProperty(ref _iconAnimationStyle, value);
    }

    /// <summary>
    /// Animation style used for the indicator.
    /// </summary>
    public TabSelectionAnimationStyle IndicatorAnimationStyle
    {
        get => _indicatorAnimationStyle;
        set => SetProperty(ref _indicatorAnimationStyle, value);
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

    /// <summary>
    /// Reselect behavior for active tab taps.
    /// </summary>
    public TabReselectBehavior ReselectBehavior
    {
        get => _reselectBehavior;
        set => SetProperty(ref _reselectBehavior, value);
    }

    /// <summary>
    /// Optional custom tab item template.
    /// </summary>
    public DataTemplate? TabItemTemplate
    {
        get => _tabItemTemplate;
        set => SetProperty(ref _tabItemTemplate, value);
    }

    /// <summary>
    /// Layout mode for the tab items.
    /// </summary>
    public TabLayoutMode TabLayoutMode
    {
        get => _tabLayoutMode;
        set => SetProperty(ref _tabLayoutMode, value);
    }

    /// <summary>
    /// Preset visual style for the tab UI.
    /// </summary>
    public TabVisualStyle VisualStyle
    {
        get => _visualStyle;
        set => SetProperty(ref _visualStyle, value);
    }

    /// <summary>
    /// Minimum tab count before Auto mode switches to scrollable.
    /// </summary>
    public int ScrollableThreshold
    {
        get => _scrollableThreshold;
        set => SetProperty(ref _scrollableThreshold, value);
    }

    /// <summary>
    /// Fixed width for tab items (use -1 for auto).
    /// </summary>
    public double TabItemWidth
    {
        get => _tabItemWidth;
        set => SetProperty(ref _tabItemWidth, value);
    }

    /// <summary>
    /// Minimum width for tab items.
    /// </summary>
    public double TabItemMinWidth
    {
        get => _tabItemMinWidth;
        set => SetProperty(ref _tabItemMinWidth, value);
    }

    /// <summary>
    /// Corner radius for the tab bar container.
    /// </summary>
    public CornerRadius TabBarCornerRadius
    {
        get => _tabBarCornerRadius;
        set => SetProperty(ref _tabBarCornerRadius, value);
    }

    /// <summary>
    /// Border color for the tab bar container.
    /// </summary>
    public Color BorderColor
    {
        get => _borderColor;
        set => SetProperty(ref _borderColor, value);
    }

    /// <summary>
    /// Border thickness for the tab bar container.
    /// </summary>
    public double BorderThickness
    {
        get => _borderThickness;
        set => SetProperty(ref _borderThickness, value);
    }

    /// <summary>
    /// Shadow applied to the tab bar container.
    /// </summary>
    public MauiShadow? TabBarShadow
    {
        get => _tabBarShadow;
        set => SetProperty(ref _tabBarShadow, value);
    }

    /// <summary>
    /// Fixed width for the indicator (use -1 for auto).
    /// </summary>
    public double IndicatorWidth
    {
        get => _indicatorWidth;
        set => SetProperty(ref _indicatorWidth, value);
    }

    /// <summary>
    /// Horizontal alignment for the indicator.
    /// </summary>
    public LayoutOptions IndicatorHorizontalOptions
    {
        get => _indicatorHorizontalOptions;
        set => SetProperty(ref _indicatorHorizontalOptions, value);
    }

    /// <summary>
    /// Corner radius for the indicator.
    /// </summary>
    public float IndicatorCornerRadius
    {
        get => _indicatorCornerRadius;
        set => SetProperty(ref _indicatorCornerRadius, value);
    }

    /// <summary>
    /// Enables keyboard navigation on desktop platforms.
    /// </summary>
    public bool EnableKeyboardNavigation
    {
        get => _enableKeyboardNavigation;
        set => SetProperty(ref _enableKeyboardNavigation, value);
    }

    /// <summary>
    /// Optional factory used to create navigation pages for all tabs.
    /// </summary>
    public Func<Page, NavigationPage>? NavigationPageFactory
    {
        get => _navigationPageFactory;
        set => SetProperty(ref _navigationPageFactory, value);
    }

    /// <summary>
    /// Default navigation bar visibility.
    /// </summary>
    public bool? NavigationBarVisible
    {
        get => _navigationBarVisible;
        set => SetProperty(ref _navigationBarVisible, value);
    }

    /// <summary>
    /// Default navigation bar background color.
    /// </summary>
    public Color? NavigationBarBackgroundColor
    {
        get => _navigationBarBackgroundColor;
        set => SetProperty(ref _navigationBarBackgroundColor, value);
    }

    /// <summary>
    /// Default navigation bar text color.
    /// </summary>
    public Color? NavigationBarTextColor
    {
        get => _navigationBarTextColor;
        set => SetProperty(ref _navigationBarTextColor, value);
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
