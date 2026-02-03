using CustomTabs.Sample.Services;
using Plugin.Maui.CustomTabs.Models;

namespace CustomTabs.Sample.Pages;

/// <summary>
/// Settings tab root page.
/// </summary>
public partial class SettingsPage : ContentPage
{
    private readonly SimpleLocalizationService _localization;
    private bool _subscribed;

    /// <summary>
    /// Creates the settings page.
    /// </summary>
    public SettingsPage(CustomTabsOptions options, SimpleLocalizationService localization)
    {
        _localization = localization;
        InitializeComponent();
        BindingContext = options;

        SubscribeLocalization();
        UpdateLanguageLabel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SubscribeLocalization();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        UnsubscribeLocalization();
    }

    private void OnEnglishClicked(object sender, EventArgs e)
    {
        _localization.SetLanguage("en");
    }

    private void OnFrenchClicked(object sender, EventArgs e)
    {
        _localization.SetLanguage("fr");
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        UpdateLanguageLabel();
    }

    private void SubscribeLocalization()
    {
        if (_subscribed)
        {
            return;
        }

        _localization.LanguageChanged += OnLanguageChanged;
        _subscribed = true;
    }

    private void UnsubscribeLocalization()
    {
        if (!_subscribed)
        {
            return;
        }

        _localization.LanguageChanged -= OnLanguageChanged;
        _subscribed = false;
    }

    private void UpdateLanguageLabel()
    {
        LanguageLabel.Text = $"Current: {_localization.CurrentLanguage.ToUpperInvariant()}";
    }

    private async void OnPushDetailsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DetailPage("Settings details"));
    }
}
