using System.Linq;
using CustomTabs.Prism.Sample.Services;
using Microsoft.Maui.Controls;
using Plugin.Maui.CustomTabs.Pages;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace CustomTabs.Prism.Sample.ViewModels;

/// <summary>
/// View model for the home page.
/// </summary>
public sealed class HomePageViewModel : BindableBase
{
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Command to push details.
    /// </summary>
    public DelegateCommand PushDetailsCommand { get; }

    /// <summary>
    /// Command to select the search tab via Prism navigation.
    /// </summary>
    public DelegateCommand SelectSearchTabCommand { get; }

    /// <summary>
    /// Command to select the settings tab via Prism navigation.
    /// </summary>
    public DelegateCommand SelectSettingsTabCommand { get; }

    /// <summary>
    /// Creates the home view model.
    /// </summary>
    public HomePageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        PushDetailsCommand = new DelegateCommand(async () =>
            await SafeExecution.RunAsync(
                () => NavigateToDetailAsync("Home details"),
                "HomePageViewModel.PushDetailsCommand"));

        SelectSearchTabCommand = new DelegateCommand(async () =>
            await SafeExecution.RunAsync(
                () => SelectTabAsync("search"),
                "HomePageViewModel.SelectSearchTabCommand"));

        SelectSettingsTabCommand = new DelegateCommand(async () =>
            await SafeExecution.RunAsync(
                () => SelectTabAsync("settings"),
                "HomePageViewModel.SelectSettingsTabCommand"));
    }

    private Task NavigateToDetailAsync(string title)
    {
        var parameters = new NavigationParameters
        {
            { "title", title }
        };

        return _navigationService.NavigateAsync("DetailPage", parameters);
    }

    private async Task SelectTabAsync(string tabKey)
    {
        var result = await _navigationService.SelectTabAsync(tabKey);
        if (result.Success)
        {
            return;
        }

        if (TrySelectTabLocally(tabKey))
        {
            return;
        }

        if (result.Exception != null)
        {
            SampleExceptionHandler.Report(result.Exception, "HomePageViewModel.SelectTabAsync");
        }
    }

    private static bool TrySelectTabLocally(string tabKey)
    {
        var page = Application.Current?.MainPage;
        if (page is NavigationPage navigationPage)
        {
            page = navigationPage.CurrentPage;
        }

        if (page is CustomTabsHostPage hostPage)
        {
            hostPage.SelectedTabKey = tabKey;
            return true;
        }

        var windowPage = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (windowPage is NavigationPage windowNav)
        {
            windowPage = windowNav.CurrentPage;
        }

        if (windowPage is CustomTabsHostPage windowHost)
        {
            windowHost.SelectedTabKey = tabKey;
            return true;
        }

        return false;
    }
}
