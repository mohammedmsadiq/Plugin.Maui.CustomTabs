using CustomTabs.Prism.Sample.Services;
using Plugin.Maui.CustomTabs.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace CustomTabs.Prism.Sample.ViewModels;

/// <summary>
/// View model for the settings page.
/// </summary>
public sealed class SettingsPageViewModel : BindableBase
{
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Options applied to the tab bar.
    /// </summary>
    public CustomTabsOptions Options { get; }

    /// <summary>
    /// Command to push details.
    /// </summary>
    public DelegateCommand PushDetailsCommand { get; }

    /// <summary>
    /// Creates the settings view model.
    /// </summary>
    public SettingsPageViewModel(CustomTabsOptions options, INavigationService navigationService)
    {
        Options = options ?? new CustomTabsOptions();
        _navigationService = navigationService;

        PushDetailsCommand = new DelegateCommand(async () =>
            await SafeExecution.RunAsync(
                () => NavigateToDetailAsync("Settings details"),
                "SettingsPageViewModel.PushDetailsCommand"));
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
