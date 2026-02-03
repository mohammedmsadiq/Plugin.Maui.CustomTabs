using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;

namespace Plugin.Maui.CustomTabs.Animations;

/// <summary>
/// Animations used by the custom tab bar.
/// </summary>
public static class TabAnimations
{
    /// <summary>
    /// Animates the selected tab icon and underline.
    /// </summary>
    public static async Task AnimateSelectionAsync(VisualElement? icon, VisualElement? underline, TimeSpan duration)
    {
        if (icon == null)
        {
            return;
        }

        var halfDuration = (uint)Math.Max(1, duration.TotalMilliseconds / 2);
        await icon.ScaleTo(1.10, halfDuration, Easing.CubicOut);
        await icon.ScaleTo(1.0, halfDuration, Easing.CubicIn);

        if (underline != null)
        {
            await underline.ScaleTo(1.05, halfDuration, Easing.CubicOut);
            await underline.ScaleTo(1.0, halfDuration, Easing.CubicIn);
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
}
