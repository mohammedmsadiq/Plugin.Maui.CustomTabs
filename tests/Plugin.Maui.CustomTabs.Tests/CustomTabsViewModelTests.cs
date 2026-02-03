#if ANDROID || IOS
using Microsoft.Maui.Controls;
using NUnit.Framework;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.ViewModels;

namespace Plugin.Maui.CustomTabs.Tests;

[TestFixture]
public sealed class CustomTabsViewModelTests
{
    [Test]
    public void InitializesSelectedTab_FromDefaultKey()
    {
        var viewModel = CreateViewModel();

        Assert.That(viewModel.SelectedTab?.Key, Is.EqualTo("home"));
        Assert.That(viewModel.SelectedIndex, Is.EqualTo(0));
    }

    [Test]
    public void SelectTabByKey_RaisesSelectedTabChanged()
    {
        var viewModel = CreateViewModel();
        TabSelectionChangedEventArgs? received = null;
        viewModel.SelectedTabChanged += (_, args) => received = args;

        viewModel.SelectTabByKey("settings");

        Assert.That(viewModel.SelectedTab?.Key, Is.EqualTo("settings"));
        Assert.That(viewModel.SelectedIndex, Is.EqualTo(2));
        Assert.That(received, Is.Not.Null);
        Assert.That(received?.SelectedKey, Is.EqualTo("settings"));
    }

    [Test]
    public void TrySelectTab_BlocksDisabledTabs()
    {
        var viewModel = CreateViewModel();
        var disabledTab = viewModel.Tabs[1];
        disabledTab.IsEnabled = false;

        var result = viewModel.TrySelectTab(disabledTab, TabSelectionOrigin.User, allowReselect: false);

        Assert.That(result, Is.EqualTo(TabSelectionResult.Blocked));
        Assert.That(viewModel.SelectedTab?.Key, Is.EqualTo("home"));
    }

    [Test]
    public void TrySelectTab_Reselect_ReturnsReselected()
    {
        var viewModel = CreateViewModel();
        var current = viewModel.SelectedTab!;

        var result = viewModel.TrySelectTab(current, TabSelectionOrigin.User, allowReselect: true);

        Assert.That(result, Is.EqualTo(TabSelectionResult.Reselected));
    }

    [Test]
    public void CanSelectTab_CanBlockSelection()
    {
        var viewModel = CreateViewModel();
        viewModel.CanSelectTab = tab => tab.Key != "search";

        viewModel.SelectTabByKey("search");

        Assert.That(viewModel.SelectedTab?.Key, Is.EqualTo("home"));
    }

    private static CustomTabsViewModel CreateViewModel()
    {
        var options = new CustomTabsOptions();
        var tabs = new[]
        {
            new CustomTabItem("home", "H", () => new ContentPage()),
            new CustomTabItem("search", "S", () => new ContentPage()),
            new CustomTabItem("settings", "T", () => new ContentPage())
        };

        return new CustomTabsViewModel(tabs, "home", options);
    }
}
#endif
