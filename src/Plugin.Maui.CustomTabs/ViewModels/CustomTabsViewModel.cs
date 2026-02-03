using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Plugin.Maui.CustomTabs.Models;

namespace Plugin.Maui.CustomTabs.ViewModels;

/// <summary>
/// View model for the custom tabs host page.
/// </summary>
public sealed class CustomTabsViewModel : INotifyPropertyChanged
{
    private CustomTabItem? _selectedTab;
    private Page? _selectedPage;

    /// <summary>
    /// Raised when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Creates a new view model instance.
    /// </summary>
    /// <param name="tabs">Tabs to display.</param>
    /// <param name="defaultKey">Default selected tab key.</param>
    /// <param name="options">Options for the tab bar.</param>
    public CustomTabsViewModel(IEnumerable<CustomTabItem> tabs, string defaultKey, CustomTabsOptions options)
    {
        if (tabs == null)
        {
            throw new ArgumentNullException(nameof(tabs));
        }

        Options = options ?? throw new ArgumentNullException(nameof(options));
        Tabs = new ObservableCollection<CustomTabItem>(tabs);
        SelectTabCommand = new Command<CustomTabItem>(SelectTab);

        var defaultTab = Tabs.FirstOrDefault(t => t.Key == defaultKey) ?? Tabs.FirstOrDefault();
        if (defaultTab != null)
        {
            SelectTab(defaultTab);
        }
    }

    /// <summary>
    /// Collection of tabs bound to the UI.
    /// </summary>
    public ObservableCollection<CustomTabItem> Tabs { get; }

    /// <summary>
    /// Options applied to the tab bar.
    /// </summary>
    public CustomTabsOptions Options { get; }

    /// <summary>
    /// Currently selected tab.
    /// </summary>
    public CustomTabItem? SelectedTab
    {
        get => _selectedTab;
        private set => SetProperty(ref _selectedTab, value);
    }

    /// <summary>
    /// Currently visible page.
    /// </summary>
    public Page? SelectedPage
    {
        get => _selectedPage;
        private set => SetProperty(ref _selectedPage, value);
    }

    /// <summary>
    /// Command used when a tab is selected.
    /// </summary>
    public ICommand SelectTabCommand { get; }

    /// <summary>
    /// Selects a tab by key.
    /// </summary>
    public void SelectTabByKey(string key)
    {
        var tab = Tabs.FirstOrDefault(t => t.Key == key);
        if (tab != null)
        {
            SelectTab(tab);
        }
    }

    /// <summary>
    /// Refreshes all tab titles by notifying bindings.
    /// </summary>
    public void RefreshTitles()
    {
        foreach (var tab in Tabs)
        {
            tab.NotifyTitleChanged();
        }
    }

    private void SelectTab(CustomTabItem? tab)
    {
        if (tab == null || ReferenceEquals(tab, SelectedTab))
        {
            return;
        }

        SelectedTab = tab;
        SelectedPage = tab.GetOrCreateNavigationPage();
    }

    private bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(backingStore, value))
        {
            return false;
        }

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
