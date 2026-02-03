using CustomTabs.Sample.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace CustomTabs.Sample.ViewModels;

/// <summary>
/// View model for the login page.
/// </summary>
public sealed class LoginPageViewModel : BindableBase
{
    private readonly INavigationService _navigationService;
    private readonly IDemoAuthService _authService;

    /// <summary>
    /// Command that simulates login.
    /// </summary>
    public DelegateCommand SignInCommand { get; }

    /// <summary>
    /// Creates the login view model.
    /// </summary>
    public LoginPageViewModel(INavigationService navigationService, IDemoAuthService authService)
    {
        _navigationService = navigationService;
        _authService = authService;
        SignInCommand = new DelegateCommand(async () =>
        {
            _authService.SetLoggedIn(true);
            await _navigationService.NavigateAsync("/MainTabsPage");
        });
    }
}
