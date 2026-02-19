using CustomTabs.Prism.Sample.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace CustomTabs.Prism.Sample.ViewModels;

/// <summary>
/// View model for the search page.
/// </summary>
public sealed class SearchPageViewModel : BindableBase
{
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Command to push details.
    /// </summary>
    public DelegateCommand PushDetailsCommand { get; }

    /// <summary>
    /// Creates the search view model.
    /// </summary>
    public SearchPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        PushDetailsCommand = new DelegateCommand(async () =>
            await SafeExecution.RunAsync(
                () => NavigateToDetailAsync("Search details"),
                "SearchPageViewModel.PushDetailsCommand"));
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
