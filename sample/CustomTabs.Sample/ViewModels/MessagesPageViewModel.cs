using CustomTabs.Sample.Services;
using Plugin.Maui.CustomTabs.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace CustomTabs.Sample.ViewModels;

/// <summary>
/// View model for the messages page.
/// </summary>
public sealed class MessagesPageViewModel : BindableBase
{
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Badge backing the message count.
    /// </summary>
    public CustomTabBadge Badge { get; }

    /// <summary>
    /// Command to increment the badge count.
    /// </summary>
    public DelegateCommand IncrementCommand { get; }

    /// <summary>
    /// Command to decrement the badge count.
    /// </summary>
    public DelegateCommand DecrementCommand { get; }

    /// <summary>
    /// Command to push details.
    /// </summary>
    public DelegateCommand PushDetailsCommand { get; }

    /// <summary>
    /// Creates the messages view model.
    /// </summary>
    public MessagesPageViewModel(INavigationService navigationService, CustomTabBadge badge)
    {
        _navigationService = navigationService;
        Badge = badge;

        IncrementCommand = new DelegateCommand(() =>
            SafeExecution.Run(() => Badge.Count += 1, "MessagesPageViewModel.IncrementCommand"));

        DecrementCommand = new DelegateCommand(() =>
            SafeExecution.Run(() =>
            {
                if (Badge.Count > 0)
                {
                    Badge.Count -= 1;
                }
            }, "MessagesPageViewModel.DecrementCommand"));

        PushDetailsCommand = new DelegateCommand(async () =>
            await SafeExecution.RunAsync(
                () => NavigateToDetailAsync("Message details"),
                "MessagesPageViewModel.PushDetailsCommand"));
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
