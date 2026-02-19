using CustomTabs.Sample.Services;
using Prism.Mvvm;
using Prism.Navigation;
using System.Diagnostics;
using Microsoft.Maui.ApplicationModel;
using System.Threading;

namespace CustomTabs.Sample.ViewModels;

/// <summary>
/// View model that decides which page to show after the splash screen.
/// </summary>
public sealed class SplashPageViewModel : BindableBase, IInitializeAsync
{
    private readonly INavigationService _navigationService;
    private readonly IDemoAuthService _authService;
    private string? _errorMessage;
    private int _navigationTriggered;

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
        await SafeExecution.RunAsync(async () =>
        {
            Debug.WriteLine("[Sample] SplashPageViewModel.InitializeAsync starting.");
            var isLoggedIn = await _authService.IsLoggedInAsync();
            Debug.WriteLine($"[Sample] Auth status: {isLoggedIn} (before demo override).");
            isLoggedIn = true; // FOR DEMO PURPOSES ONLY
            // var target = isLoggedIn ? "/NavigationPage/MainTabsPage" : "/NavigationPage/LoginPage";
            var target = "/NavigationPage/MainTabsPage";
            Debug.WriteLine($"[Sample] Navigating to {target}.");

            // Prism is still completing the splash navigation pipeline here.
            // Defer the next navigation so we don't block or re-enter the current flow.
            if (Interlocked.Exchange(ref _navigationTriggered, 1) == 0)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                    SafeFireAndForget(NavigateToTargetAsync(target), "SplashPageViewModel.NavigateToTargetAsync"));
            }
        }, "SplashPageViewModel.InitializeAsync");
    }

    private async Task NavigateToTargetAsync(string target)
    {
        await Task.Delay(50);

        var result = await _navigationService.NavigateAsync(target);
        if (!result.Success)
        {
            var message = result.Exception?.ToString() ?? "Navigation failed.";
            Debug.WriteLine(message);
            ErrorMessage = message;
            Debug.WriteLine("[Sample] Splash navigation failed.");
            return;
        }

        Debug.WriteLine("[Sample] Splash navigation succeeded.");
    }

    private static void SafeFireAndForget(Task task, string context)
    {
        if (task.IsCompletedSuccessfully)
        {
            return;
        }

        _ = ObserveTaskAsync(task, context);
    }

    private static async Task ObserveTaskAsync(Task task, string context)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            SampleExceptionHandler.Report(ex, context);
        }
    }
}
