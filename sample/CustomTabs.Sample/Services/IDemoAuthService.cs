namespace CustomTabs.Sample.Services;

/// <summary>
/// Simple authentication service for demo navigation.
/// </summary>
public interface IDemoAuthService
{
    /// <summary>
    /// Returns whether the user is logged in.
    /// </summary>
    Task<bool> IsLoggedInAsync();

    /// <summary>
    /// Marks the user as logged in or out.
    /// </summary>
    void SetLoggedIn(bool isLoggedIn);
}
