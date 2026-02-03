using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Plugin.Maui.CustomTabs.Models;

namespace Plugin.Maui.CustomTabs.Animations;

/// <summary>
/// Animations used by the custom tab bar.
/// </summary>
public static class TabAnimations
{
    /// <summary>
    /// Animates the selected tab icon and indicator.
    /// </summary>
    public static async Task AnimateSelectionAsync(
        VisualElement? icon,
        VisualElement? indicator,
        TimeSpan duration,
        TabSelectionAnimationStyle iconStyle,
        TabSelectionAnimationStyle indicatorStyle)
    {
        if (icon != null)
        {
            await AnimateElementAsync(icon, duration, iconStyle, 1.10);
        }

        if (indicator != null)
        {
            await AnimateElementAsync(indicator, duration, indicatorStyle, 1.05);
        }
    }

    /// <summary>
    /// Performs a light haptic feedback if supported.
    /// </summary>
    public static void TryPerformHaptic()
    {
        try
        {
            if (HapticFeedback.Default.IsSupported)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
        }
        catch
        {
            // Ignore haptic failures on unsupported platforms.
        }
    }

    private static async Task AnimateElementAsync(VisualElement element, TimeSpan duration, TabSelectionAnimationStyle style, double scaleTarget)
    {
        if (style == TabSelectionAnimationStyle.None)
        {
            return;
        }

        var halfDuration = (uint)Math.Max(1, duration.TotalMilliseconds / 2);

        switch (style)
        {
            case TabSelectionAnimationStyle.Scale:
                await element.ScaleTo(scaleTarget, halfDuration, Easing.CubicOut);
                await element.ScaleTo(1.0, halfDuration, Easing.CubicIn);
                break;
            case TabSelectionAnimationStyle.Fade:
                await element.FadeTo(0.6, halfDuration, Easing.CubicOut);
                await element.FadeTo(1.0, halfDuration, Easing.CubicIn);
                break;
            case TabSelectionAnimationStyle.None:
            default:
                break;
        }
    }
}
