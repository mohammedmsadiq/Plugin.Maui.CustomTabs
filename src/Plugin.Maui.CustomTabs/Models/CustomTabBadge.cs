using System.ComponentModel;
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
    private Color _backgroundColor = Colors.Red;
    private Color _textColor = Colors.White;

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
    /// Count to display. Values less than or equal to zero hide the count badge.
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
            }
        }
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
    /// True when the badge should be visible.
    /// </summary>
    public bool IsVisible => IsDot || Count > 0;

    /// <summary>
    /// True when the dot badge should be visible.
    /// </summary>
    public bool IsDotVisible => IsDot;

    /// <summary>
    /// True when the count badge should be visible.
    /// </summary>
    public bool IsCountVisible => !IsDot && Count > 0;

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
