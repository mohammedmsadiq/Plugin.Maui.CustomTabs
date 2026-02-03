using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;

namespace Plugin.Maui.CustomTabs.Models;

/// <summary>
/// Represents a badge displayed on a tab icon (dot or count).
/// </summary>
public sealed class CustomTabBadge : INotifyPropertyChanged
{
    private bool _isDot;
    private int _count;
    private int _maxCount = 99;
    private string? _text;
    private double _fontSize = 10;
    private double _minWidth = 16;
    private double _minHeight = 16;
    private float _cornerRadius = 8;
    private Color _backgroundColor = Colors.Red;
    private Color _textColor = Colors.White;
    private Color _borderColor = Colors.Transparent;
    private double _borderThickness;
    private double _dotSize = 8;

    /// <summary>
    /// Raised when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// When true, shows a dot badge instead of a count.
    /// </summary>
    public bool IsDot
    {
        get => _isDot;
        set
        {
            if (SetProperty(ref _isDot, value))
            {
                OnPropertyChanged(nameof(IsVisible));
                OnPropertyChanged(nameof(IsDotVisible));
                OnPropertyChanged(nameof(IsCountVisible));
            }
        }
    }

    /// <summary>
    /// Count to display. Values less than or equal to zero hide the count badge unless Text is set.
    /// </summary>
    public int Count
    {
        get => _count;
        set
        {
            if (SetProperty(ref _count, value))
            {
                OnPropertyChanged(nameof(IsVisible));
                OnPropertyChanged(nameof(IsCountVisible));
                OnPropertyChanged(nameof(DisplayText));
            }
        }
    }

    /// <summary>
    /// Maximum count before showing a + suffix. Set to 0 or less to disable.
    /// </summary>
    public int MaxCount
    {
        get => _maxCount;
        set
        {
            if (SetProperty(ref _maxCount, value))
            {
                OnPropertyChanged(nameof(DisplayText));
            }
        }
    }

    /// <summary>
    /// Optional custom text for the badge (overrides Count).
    /// </summary>
    public string? Text
    {
        get => _text;
        set
        {
            if (SetProperty(ref _text, value))
            {
                OnPropertyChanged(nameof(IsVisible));
                OnPropertyChanged(nameof(IsCountVisible));
                OnPropertyChanged(nameof(DisplayText));
            }
        }
    }

    /// <summary>
    /// Font size for the count badge.
    /// </summary>
    public double FontSize
    {
        get => _fontSize;
        set => SetProperty(ref _fontSize, value);
    }

    /// <summary>
    /// Minimum width for the count badge.
    /// </summary>
    public double MinWidth
    {
        get => _minWidth;
        set => SetProperty(ref _minWidth, value);
    }

    /// <summary>
    /// Minimum height for the count badge.
    /// </summary>
    public double MinHeight
    {
        get => _minHeight;
        set => SetProperty(ref _minHeight, value);
    }

    /// <summary>
    /// Corner radius for the count badge.
    /// </summary>
    public float CornerRadius
    {
        get => _cornerRadius;
        set => SetProperty(ref _cornerRadius, value);
    }

    /// <summary>
    /// Background color for the badge.
    /// </summary>
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set => SetProperty(ref _backgroundColor, value);
    }

    /// <summary>
    /// Text color for the count badge.
    /// </summary>
    public Color TextColor
    {
        get => _textColor;
        set => SetProperty(ref _textColor, value);
    }

    /// <summary>
    /// Border color for the count badge.
    /// </summary>
    public Color BorderColor
    {
        get => _borderColor;
        set => SetProperty(ref _borderColor, value);
    }

    /// <summary>
    /// Border thickness for the count badge.
    /// </summary>
    public double BorderThickness
    {
        get => _borderThickness;
        set => SetProperty(ref _borderThickness, value);
    }

    /// <summary>
    /// Size of the dot badge.
    /// </summary>
    public double DotSize
    {
        get => _dotSize;
        set => SetProperty(ref _dotSize, value);
    }

    /// <summary>
    /// True when the badge should be visible.
    /// </summary>
    public bool IsVisible => IsDot || Count > 0 || !string.IsNullOrWhiteSpace(Text);

    /// <summary>
    /// True when the dot badge should be visible.
    /// </summary>
    public bool IsDotVisible => IsDot;

    /// <summary>
    /// True when the count badge should be visible.
    /// </summary>
    public bool IsCountVisible => !IsDot && (Count > 0 || !string.IsNullOrWhiteSpace(Text));

    /// <summary>
    /// Display text for the count badge.
    /// </summary>
    public string DisplayText
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                return Text ?? string.Empty;
            }

            if (Count <= 0)
            {
                return string.Empty;
            }

            if (MaxCount > 0 && Count > MaxCount)
            {
                return string.Concat(MaxCount.ToString(CultureInfo.InvariantCulture), "+");
            }

            return Count.ToString(CultureInfo.InvariantCulture);
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
