using CustomTabs.Sample.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace CustomTabs.Sample.ViewModels;

/// <summary>
/// View model for the profile page.
/// </summary>
public sealed class ProfilePageViewModel : BindableBase
{
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Command to push details.
    /// </summary>
    public DelegateCommand PushDetailsCommand { get; }

    /// <summary>
    /// Creates the profile view model.
    /// </summary>
    public ProfilePageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        PushDetailsCommand = new DelegateCommand(async () =>
            await SafeExecution.RunAsync(
                () => NavigateToDetailAsync("Profile details"),
                "ProfilePageViewModel.PushDetailsCommand"));
    }

    private Task NavigateToDetailAsync(string title)
    {
        var parameters = new NavigationParameters
        {
            { "title", title }
        };

        return _navigationService.NavigateAsync("DetailPage", parameters);
    }
}
