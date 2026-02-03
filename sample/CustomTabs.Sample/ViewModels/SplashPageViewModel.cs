using CustomTabs.Sample.Services;
using Prism.Mvvm;
using Prism.Navigation;
using System.Diagnostics;
using Microsoft.Maui.ApplicationModel;

namespace CustomTabs.Sample.ViewModels;

/// <summary>
/// View model that decides which page to show after the splash screen.
/// </summary>
public sealed class SplashPageViewModel : BindableBase, IInitializeAsync
{
    private readonly INavigationService _navigationService;
    private readonly IDemoAuthService _authService;
    private string? _errorMessage;

    /// <summary>
    /// Creates the splash view model.
    /// </summary>
    public SplashPageViewModel(INavigationService navigationService, IDemoAuthService authService)
    {
        _navigationService = navigationService;
        _authService = authService;
    }

    /// <summary>
    /// Error details shown when startup navigation fails.
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    /// <inheritdoc />
    public async Task InitializeAsync(INavigationParameters parameters)
    {
        Debug.WriteLine("[Sample] SplashPageViewModel.InitializeAsync starting.");
        var isLoggedIn = await _authService.IsLoggedInAsync();
        Debug.WriteLine($"[Sample] Auth status: {isLoggedIn} (before demo override).");
        isLoggedIn = true; // FOR DEMO PURPOSES ONLY
        // var target = isLoggedIn ? "/MainTabsPage" : "/NavigationPage/LoginPage";
        var target = "/MainTabsPage";
        Debug.WriteLine($"[Sample] Scheduling navigation to {target} after init completes.");

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await Task.Delay(50);
                Debug.WriteLine($"[Sample] Navigating to {target}.");
                var result = await _navigationService.NavigateAsync(target);
                if (!result.Success)
                {
                    var message = result.Exception?.ToString() ?? "Navigation failed.";
                    System.Diagnostics.Debug.WriteLine(message);
                    ErrorMessage = message;
                    Debug.WriteLine("[Sample] Splash navigation failed.");
                    return;
                }

                Debug.WriteLine("[Sample] Splash navigation succeeded.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                ErrorMessage = ex.Message;
            }
        });
    }
}
