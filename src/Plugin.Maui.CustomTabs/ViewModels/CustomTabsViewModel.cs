using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Services;
using System.Diagnostics;

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
    /// Raised when the selected tab changes.
    /// </summary>
    public event EventHandler<TabSelectionChangedEventArgs>? SelectedTabChanged;

    /// <summary>
    /// Raised before a tab selection occurs. Set Cancel to true to block.
    /// </summary>
    public event EventHandler<TabSelectionRequestedEventArgs>? TabSelectionRequested;

    /// <summary>
    /// Optional predicate to allow or block selection.
    /// </summary>
    public Func<CustomTabItem, bool>? CanSelectTab { get; set; }

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
        SelectTabCommand = new Command<CustomTabItem>(tab =>
            SafeFireAndForget(
                HandleSelectionAsync(tab, TabSelectionOrigin.User, allowReselect: true),
                "CustomTabsViewModel.SelectTabCommand"));

        Tabs.CollectionChanged += OnTabsCollectionChanged;
        foreach (var tab in Tabs)
        {
            SubscribeTab(tab);
        }

        var defaultTab = ResolveDefaultTab(defaultKey);
        if (defaultTab != null)
        {
            Debug.WriteLine($"[CustomTabs] ViewModel default tab resolved: {defaultTab.Key}.");
            SelectTabInternal(defaultTab);
        }
        else
        {
            Debug.WriteLine("[CustomTabs] ViewModel default tab not found.");
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
    /// Currently visible content view.
    /// </summary>
    public View? SelectedContent => SelectedTab?.CurrentContent;

    /// <summary>
    /// Index of the selected tab within the tabs collection.
    /// </summary>
    public int SelectedIndex => SelectedTab == null ? -1 : Tabs.IndexOf(SelectedTab);

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
            SafeFireAndForget(
                HandleSelectionAsync(tab, TabSelectionOrigin.Programmatic, allowReselect: false),
                "CustomTabsViewModel.SelectTabByKey");
        }
    }

    /// <summary>
    /// Selects a tab by index.
    /// </summary>
    public void SelectTabByIndex(int index)
    {
        if (index < 0 || index >= Tabs.Count)
        {
            return;
        }

        SafeFireAndForget(
            HandleSelectionAsync(Tabs[index], TabSelectionOrigin.Programmatic, allowReselect: false),
            "CustomTabsViewModel.SelectTabByIndex");
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

    /// <summary>
    /// Attempts to select the tab and returns the selection result.
    /// </summary>
    public TabSelectionResult TrySelectTab(CustomTabItem tab, TabSelectionOrigin origin, bool allowReselect)
    {
        if (tab == null)
        {
            return TabSelectionResult.None;
        }

        if (!tab.IsVisible || !tab.IsEnabled)
        {
            return TabSelectionResult.Blocked;
        }

        var isReselect = ReferenceEquals(tab, SelectedTab);
        if (isReselect && !allowReselect)
        {
            return TabSelectionResult.None;
        }

        var args = new TabSelectionRequestedEventArgs(tab, SelectedTab, origin, isReselect);
        TabSelectionRequested?.Invoke(this, args);
        if (args.Cancel)
        {
            return TabSelectionResult.Blocked;
        }

        if (CanSelectTab != null && !CanSelectTab(tab))
        {
            return TabSelectionResult.Blocked;
        }

        if (isReselect)
        {
            return TabSelectionResult.Reselected;
        }

        SelectTabInternal(tab);
        return TabSelectionResult.Selected;
    }

    /// <summary>
    /// Executes configured behaviors when the selected tab is tapped again.
    /// </summary>
    public async Task HandleReselectAsync(CustomTabItem tab)
    {
        var behavior = Options.ReselectBehavior;
        if (behavior == TabReselectBehavior.None)
        {
            return;
        }

        if (behavior.HasFlag(TabReselectBehavior.PopToRoot))
        {
            var navPage = tab.NavigationPage;
            if (navPage != null && navPage.Navigation.NavigationStack.Count > 1)
            {
                await navPage.PopToRootAsync();
            }
        }

        if (behavior.HasFlag(TabReselectBehavior.ScrollToTop))
        {
            await TryScrollToTopAsync(tab);
        }

        if (behavior.HasFlag(TabReselectBehavior.Command))
        {
            var command = tab.ReselectCommand;
            var parameter = tab.ReselectCommandParameter ?? tab;
            if (command?.CanExecute(parameter) == true)
            {
                command.Execute(parameter);
            }
        }
    }

    private async Task HandleSelectionAsync(CustomTabItem? tab, TabSelectionOrigin origin, bool allowReselect)
    {
        if (tab == null)
        {
            return;
        }

        try
        {
            var result = TrySelectTab(tab, origin, allowReselect);
            if (result == TabSelectionResult.Reselected)
            {
                await HandleReselectAsync(tab);
            }
        }
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, $"CustomTabsViewModel.HandleSelectionAsync({tab.Key})");
        }
    }

    private async Task TryScrollToTopAsync(CustomTabItem tab)
    {
        var navPage = tab.NavigationPage;
        var currentPage = navPage?.CurrentPage;

        if (currentPage is ITabScrollToTop scrollable)
        {
            await scrollable.ScrollToTopAsync();
            return;
        }

        if (currentPage?.BindingContext is ITabScrollToTop scrollableContext)
        {
            await scrollableContext.ScrollToTopAsync();
        }
    }

    private void SelectTabInternal(CustomTabItem tab)
    {
        try
        {
            var previousTab = SelectedTab;
            SelectedTab = tab;
            SelectedPage = tab.GetOrCreateNavigationPage(Options);
            OnPropertyChanged(nameof(SelectedContent));
            Debug.WriteLine($"[CustomTabs] Selected tab set to {tab.Key}. NavigationPage created={SelectedPage != null}.");
            OnPropertyChanged(nameof(SelectedIndex));
            SelectedTabChanged?.Invoke(this, new TabSelectionChangedEventArgs(previousTab, SelectedTab, SelectedIndex));
        }
        catch (Exception ex)
        {
            CustomTabsExceptionReporter.Report(ex, $"CustomTabsViewModel.SelectTabInternal({tab.Key})");
        }
    }

    private CustomTabItem? ResolveDefaultTab(string? defaultKey)
    {
        if (!string.IsNullOrWhiteSpace(defaultKey))
        {
            var match = Tabs.FirstOrDefault(t => t.Key == defaultKey && t.IsVisible && t.IsEnabled);
            if (match != null)
            {
                return match;
            }
        }

        return Tabs.FirstOrDefault(t => t.IsVisible && t.IsEnabled)
            ?? Tabs.FirstOrDefault(t => t.IsVisible)
            ?? Tabs.FirstOrDefault();
    }

    private void OnTabsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (CustomTabItem tab in e.OldItems)
            {
                UnsubscribeTab(tab);
            }
        }

        if (e.NewItems != null)
        {
            foreach (CustomTabItem tab in e.NewItems)
            {
                SubscribeTab(tab);
            }
        }

        if (SelectedTab != null && !Tabs.Contains(SelectedTab))
        {
            SelectedTab = null;
            SelectedPage = null;
        }

        EnsureValidSelection();
        OnPropertyChanged(nameof(SelectedIndex));
    }

    private void SubscribeTab(CustomTabItem tab)
    {
        tab.PropertyChanged += OnTabPropertyChanged;
    }

    private void UnsubscribeTab(CustomTabItem tab)
    {
        tab.PropertyChanged -= OnTabPropertyChanged;
    }

    private void OnTabPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CustomTabItem.IsVisible) || e.PropertyName == nameof(CustomTabItem.IsEnabled))
        {
            EnsureValidSelection();
        }

        if (ReferenceEquals(sender, SelectedTab) &&
            (e.PropertyName == nameof(CustomTabItem.CurrentContent) || e.PropertyName == nameof(CustomTabItem.CurrentPage)))
        {
            OnPropertyChanged(nameof(SelectedContent));
        }
    }

    private void EnsureValidSelection()
    {
        if (SelectedTab != null && SelectedTab.IsVisible && SelectedTab.IsEnabled)
        {
            return;
        }

        var next = Tabs.FirstOrDefault(t => t.IsVisible && t.IsEnabled)
            ?? Tabs.FirstOrDefault(t => t.IsVisible)
            ?? Tabs.FirstOrDefault();

        if (next != null)
        {
            SelectTabInternal(next);
        }
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
            CustomTabsExceptionReporter.Report(ex, context);
        }
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
