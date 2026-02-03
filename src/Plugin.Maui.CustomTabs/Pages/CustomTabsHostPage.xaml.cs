using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Devices;
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Services;
using Plugin.Maui.CustomTabs.ViewModels;

namespace Plugin.Maui.CustomTabs.Pages;

/// <summary>
/// Host page that renders the selected tab content with a custom tab bar.
/// </summary>
public partial class CustomTabsHostPage : ContentPage
{
    private readonly ILocalizationRefreshService? _localizationService;
    private bool _localizationSubscribed;

    /// <summary>
    /// Creates a new host page.
    /// </summary>
    public CustomTabsHostPage(CustomTabsViewModel viewModel, ILocalizationRefreshService? localizationService)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _localizationService = localizationService;

        viewModel.Options.PropertyChanged += OnOptionsPropertyChanged;

        UpdateSafeAreaPadding();
    }

    private CustomTabsViewModel? ViewModel => BindingContext as CustomTabsViewModel;

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();
        SubscribeLocalization();
        UpdateSafeAreaPadding();
    }

    /// <inheritdoc />
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        UnsubscribeLocalization();
    }

    /// <inheritdoc />
    protected override bool OnBackButtonPressed()
    {
        if (DeviceInfo.Platform == DevicePlatform.Android && ViewModel?.SelectedTab?.NavigationPage != null)
        {
            var nav = ViewModel.SelectedTab.NavigationPage;
            if (nav.Navigation.NavigationStack.Count > 1)
            {
                _ = nav.PopAsync();
                return true;
            }
        }

        return base.OnBackButtonPressed();
    }

    private void SubscribeLocalization()
    {
        if (_localizationService == null || _localizationSubscribed)
        {
            return;
        }

        _localizationService.LanguageChanged += OnLanguageChanged;
        _localizationSubscribed = true;
        ViewModel?.RefreshTitles();
    }

    private void UnsubscribeLocalization()
    {
        if (_localizationService == null || !_localizationSubscribed)
        {
            return;
        }

        _localizationService.LanguageChanged -= OnLanguageChanged;
        _localizationSubscribed = false;
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        ViewModel?.RefreshTitles();
    }

    private void OnOptionsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CustomTabsOptions.RespectSafeArea))
        {
            UpdateSafeAreaPadding();
        }
    }

    private void UpdateSafeAreaPadding()
    {
        var viewModel = ViewModel;
        if (viewModel == null)
        {
            return;
        }

        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            this.On<iOS>().SetUseSafeArea(viewModel.Options.RespectSafeArea);
            TabBarHost.Padding = Thickness.Zero;
            return;
        }

        if (!viewModel.Options.RespectSafeArea)
        {
            TabBarHost.Padding = Thickness.Zero;
            return;
        }

        // On Android, MAUI layouts already respect system bars by default.
        // Keep padding at zero and rely on TabBarPadding for visual spacing.
        TabBarHost.Padding = Thickness.Zero;
    }
}
