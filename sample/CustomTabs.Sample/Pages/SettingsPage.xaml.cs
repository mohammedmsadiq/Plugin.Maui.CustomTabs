using CustomTabs.Sample.Services;
using Plugin.Maui.CustomTabs.Models;

namespace CustomTabs.Sample.Pages;

/// <summary>
/// Settings tab root page.
/// </summary>
public partial class SettingsPage : ContentPage
{
    private readonly SimpleLocalizationService _localization;

    /// <summary>
    /// Creates the settings page.
    /// </summary>
    public SettingsPage(CustomTabsOptions options, SimpleLocalizationService localization)
    {
        _localization = localization;
        InitializeComponent();
        BindingContext = options;

        _localization.LanguageChanged += OnLanguageChanged;
        UpdateLanguageLabel();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _localization.LanguageChanged -= OnLanguageChanged;
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

    private void UpdateLanguageLabel()
    {
        LanguageLabel.Text = $"Current: {_localization.CurrentLanguage.ToUpperInvariant()}";
    }

    private async void OnPushDetailsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DetailPage("Settings details"));
    }
}
