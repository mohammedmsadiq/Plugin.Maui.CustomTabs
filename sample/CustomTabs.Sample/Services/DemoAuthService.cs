namespace CustomTabs.Sample.Services;

/// <summary>
/// In-memory auth state used for demo navigation.
/// </summary>
public sealed class DemoAuthService : IDemoAuthService
{
    private bool _isLoggedIn;

    /// <inheritdoc />
    public Task<bool> IsLoggedInAsync() => Task.FromResult(_isLoggedIn);

    /// <inheritdoc />
    public void SetLoggedIn(bool isLoggedIn)
    {
        _isLoggedIn = isLoggedIn;
    }
}
