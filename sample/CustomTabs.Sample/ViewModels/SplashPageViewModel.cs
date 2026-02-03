using CustomTabs.Sample.Services;
using Prism.Mvvm;
using Prism.Navigation;

namespace CustomTabs.Sample.ViewModels;

/// <summary>
/// View model that decides which page to show after the splash screen.
/// </summary>
public sealed class SplashPageViewModel : BindableBase, IInitializeAsync
{
    private readonly INavigationService _navigationService;
    private readonly IDemoAuthService _authService;

    /// <summary>
    /// Creates the splash view model.
    /// </summary>
    public SplashPageViewModel(INavigationService navigationService, IDemoAuthService authService)
    {
        _navigationService = navigationService;
        _authService = authService;
    }

    /// <inheritdoc />
    public async Task InitializeAsync(INavigationParameters parameters)
    {
        var isLoggedIn = await _authService.IsLoggedInAsync();
        var target = isLoggedIn ? "/MainTabsPage" : "/NavigationPage/LoginPage";
        await _navigationService.NavigateAsync(target);
    }
}
