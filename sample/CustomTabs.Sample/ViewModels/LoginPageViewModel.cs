using CustomTabs.Sample.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Threading.Tasks;

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
        SignInCommand = new DelegateCommand(() =>
            SafeFireAndForget(ExecuteSignInAsync(), "LoginPageViewModel.SignInCommand"));
    }

    private Task ExecuteSignInAsync()
    {
        return SafeExecution.RunAsync(async () =>
        {
            _authService.SetLoggedIn(true);
            await _navigationService.NavigateAsync("/MainTabsPage");
        }, "LoginPageViewModel.SignInCommand");
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
