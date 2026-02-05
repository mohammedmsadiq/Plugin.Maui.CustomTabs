using System.Diagnostics;

namespace Plugin.Maui.CustomTabs.Services;

/// <summary>
/// Default exception handler that writes details to debug output.
/// </summary>
public sealed class DebugCustomTabsExceptionHandler : ICustomTabsExceptionHandler
{
    /// <inheritdoc />
    public void Handle(Exception exception, string context)
    {
        Debug.WriteLine($"[CustomTabs][Error] {context}: {exception}");
    }
}
