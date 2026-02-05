using CustomTabs.Sample.Services;
using Plugin.Maui.CustomTabs.Models;

namespace CustomTabs.Sample.Pages;

/// <summary>
/// Settings tab root page.
/// </summary>
public partial class SettingsPage : ContentPage
{
    private static readonly List<TabStyleOption> SupportedStyleOptions = new()
    {
        new TabStyleOption("1. Classic Bottom", TabVisualStyle.ClassicBottom),
        new TabStyleOption("2. Top Underline", TabVisualStyle.TopUnderline),
        new TabStyleOption("3. Segmented", TabVisualStyle.Segmented),
        new TabStyleOption("4. Floating", TabVisualStyle.Floating),
        new TabStyleOption("6. Icon Only", TabVisualStyle.IconOnly),
        new TabStyleOption("8. Compact", TabVisualStyle.Compact),
        new TabStyleOption("9. Minimal", TabVisualStyle.Minimal),
        new TabStyleOption("10. Pills", TabVisualStyle.Pills)
    };
    private static readonly List<TabLayoutModeOption> SupportedLayoutModes = new()
    {
        new TabLayoutModeOption("Layout: Fixed", TabLayoutMode.Fixed),
        new TabLayoutModeOption("Layout: Scrollable", TabLayoutMode.Scrollable),
        new TabLayoutModeOption("Layout: Auto", TabLayoutMode.Auto)
    };
    private static readonly List<ReselectBehaviorOption> SupportedReselectBehaviors = new()
    {
        new ReselectBehaviorOption("Reselect: None", TabReselectBehavior.None),
        new ReselectBehaviorOption("Reselect: Pop To Root", TabReselectBehavior.PopToRoot),
        new ReselectBehaviorOption("Reselect: Pop + Scroll Top", TabReselectBehavior.PopToRoot | TabReselectBehavior.ScrollToTop),
        new ReselectBehaviorOption("Reselect: Command", TabReselectBehavior.Command)
    };

    private readonly CustomTabsOptions _options;
    private readonly SimpleLocalizationService _localization;
    private bool _subscribed;
    private bool _isSyncing;

    /// <summary>
    /// Creates the settings page.
    /// </summary>
    public SettingsPage(CustomTabsOptions options, SimpleLocalizationService localization)
    {
        _options = options ?? new CustomTabsOptions();
        _localization = localization ?? new SimpleLocalizationService();
        InitializeComponent();
        BindingContext = _options;

        if (VisualStylePicker != null)
        {
            VisualStylePicker.ItemsSource = SupportedStyleOptions;
        }
        if (LayoutModePicker != null)
        {
            LayoutModePicker.ItemsSource = SupportedLayoutModes;
        }
        if (ReselectBehaviorPicker != null)
        {
            ReselectBehaviorPicker.ItemsSource = SupportedReselectBehaviors;
        }
        SyncSelectedStyle();
        SyncAdvancedControls();

        SubscribeLocalization();
        UpdateLanguageLabel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SyncSelectedStyle();
        SyncAdvancedControls();
        SubscribeLocalization();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        UnsubscribeLocalization();
    }

    private void OnVisualStyleChanged(object? sender, EventArgs e)
    {
        if (_isSyncing)
        {
            return;
        }

        SafeExecution.Run(() =>
        {
            if (VisualStylePicker?.SelectedItem is not TabStyleOption option)
            {
                return;
            }

            _options.VisualStyle = option.Style;
        }, "SettingsPage.OnVisualStyleChanged");
    }

    private void OnLayoutModeChanged(object? sender, EventArgs e)
    {
        if (_isSyncing)
        {
            return;
        }

        SafeExecution.Run(() =>
        {
            if (LayoutModePicker?.SelectedItem is not TabLayoutModeOption option)
            {
                return;
            }

            _options.TabLayoutMode = option.Mode;
        }, "SettingsPage.OnLayoutModeChanged");
    }

    private void OnReselectBehaviorChanged(object? sender, EventArgs e)
    {
        if (_isSyncing)
        {
            return;
        }

        SafeExecution.Run(() =>
        {
            if (ReselectBehaviorPicker?.SelectedItem is not ReselectBehaviorOption option)
            {
                return;
            }

            _options.ReselectBehavior = option.Behavior;
        }, "SettingsPage.OnReselectBehaviorChanged");
    }

    private void OnIconSizeChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_isSyncing)
        {
            return;
        }

        _options.IconSize = e.NewValue;
        UpdateRangeLabels();
    }

    private void OnTextSizeChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_isSyncing)
        {
            return;
        }

        _options.TextSize = e.NewValue;
        UpdateRangeLabels();
    }

    private void OnTabBarHeightChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_isSyncing)
        {
            return;
        }

        _options.TabBarHeight = e.NewValue;
        UpdateRangeLabels();
    }

    private void OnBorderThicknessChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_isSyncing)
        {
            return;
        }

        _options.BorderThickness = e.NewValue;
        UpdateRangeLabels();
    }

    private void OnCornerRadiusChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_isSyncing)
        {
            return;
        }

        _options.TabBarCornerRadius = new CornerRadius(e.NewValue);
        UpdateRangeLabels();
    }

    private void OnScrollableThresholdChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_isSyncing)
        {
            return;
        }

        _options.ScrollableThreshold = (int)Math.Round(e.NewValue);
        UpdateRangeLabels();
    }

    private void OnThemeNeutralClicked(object sender, EventArgs e)
    {
        ApplyThemePreset(
            background: "#1F2937",
            accent: "#E5E7EB",
            selected: "#F9FAFB",
            unselected: "#9CA3AF",
            border: "#374151");
    }

    private void OnThemeSlateClicked(object sender, EventArgs e)
    {
        ApplyThemePreset(
            background: "#0F172A",
            accent: "#CBD5E1",
            selected: "#E2E8F0",
            unselected: "#94A3B8",
            border: "#334155");
    }

    private void OnThemeMutedClicked(object sender, EventArgs e)
    {
        ApplyThemePreset(
            background: "#111827",
            accent: "#D1D5DB",
            selected: "#F3F4F6",
            unselected: "#9CA3AF",
            border: "#4B5563");
    }

    private void OnEnglishClicked(object sender, EventArgs e)
    {
        SafeExecution.Run(() => _localization.SetLanguage("en"), "SettingsPage.OnEnglishClicked");
    }

    private void OnFrenchClicked(object sender, EventArgs e)
    {
        SafeExecution.Run(() => _localization.SetLanguage("fr"), "SettingsPage.OnFrenchClicked");
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
        if (LanguageLabel != null)
        {
            LanguageLabel.Text = $"Current: {_localization.CurrentLanguage.ToUpperInvariant()}";
        }
    }

    private async void OnPushDetailsClicked(object sender, EventArgs e)
    {
        await SafeExecution.RunAsync(
            () => Navigation.PushAsync(new DetailPage("Settings details")),
            "SettingsPage.OnPushDetailsClicked");
    }

    private void SyncSelectedStyle()
    {
        if (VisualStylePicker == null)
        {
            return;
        }

        var match = SupportedStyleOptions.FirstOrDefault(item => item.Style == _options.VisualStyle);
        if (match != null)
        {
            VisualStylePicker.SelectedItem = match;
        }
    }

    private void SyncAdvancedControls()
    {
        _isSyncing = true;
        try
        {
            if (LayoutModePicker != null)
            {
                LayoutModePicker.SelectedItem = SupportedLayoutModes.FirstOrDefault(item => item.Mode == _options.TabLayoutMode);
            }

            if (ReselectBehaviorPicker != null)
            {
                ReselectBehaviorPicker.SelectedItem = SupportedReselectBehaviors.FirstOrDefault(item => item.Behavior == _options.ReselectBehavior);
            }

            if (IconSizeSlider != null)
            {
                IconSizeSlider.Value = _options.IconSize;
            }

            if (TextSizeSlider != null)
            {
                TextSizeSlider.Value = _options.TextSize;
            }

            if (TabBarHeightSlider != null)
            {
                TabBarHeightSlider.Value = _options.TabBarHeight;
            }

            if (BorderThicknessSlider != null)
            {
                BorderThicknessSlider.Value = _options.BorderThickness;
            }

            if (CornerRadiusSlider != null)
            {
                CornerRadiusSlider.Value = _options.TabBarCornerRadius.TopLeft;
            }

            if (ScrollableThresholdStepper != null)
            {
                ScrollableThresholdStepper.Value = _options.ScrollableThreshold;
            }

            UpdateRangeLabels();
        }
        finally
        {
            _isSyncing = false;
        }
    }

    private void ApplyThemePreset(string background, string accent, string selected, string unselected, string border)
    {
        SafeExecution.Run(() =>
        {
            _options.BackgroundColor = Color.FromArgb(background);
            _options.AccentColor = Color.FromArgb(accent);
            _options.SelectedIconColor = Color.FromArgb(selected);
            _options.UnselectedIconColor = Color.FromArgb(unselected);
            _options.SelectedTextColor = Color.FromArgb(selected);
            _options.UnselectedTextColor = Color.FromArgb(unselected);
            _options.BorderColor = Color.FromArgb(border);
            UpdateRangeLabels();
        }, "SettingsPage.ApplyThemePreset");
    }

    private void UpdateRangeLabels()
    {
        if (IconSizeValueLabel != null)
        {
            IconSizeValueLabel.Text = $"Icon size: {_options.IconSize:F0}";
        }

        if (TextSizeValueLabel != null)
        {
            TextSizeValueLabel.Text = $"Text size: {_options.TextSize:F0}";
        }

        if (TabBarHeightValueLabel != null)
        {
            TabBarHeightValueLabel.Text = $"Tab bar height: {_options.TabBarHeight:F0}";
        }

        if (BorderThicknessValueLabel != null)
        {
            BorderThicknessValueLabel.Text = $"Border thickness: {_options.BorderThickness:F1}";
        }

        if (CornerRadiusValueLabel != null)
        {
            CornerRadiusValueLabel.Text = $"Tab bar corner radius: {_options.TabBarCornerRadius.TopLeft:F0}";
        }

        if (ScrollableThresholdValueLabel != null)
        {
            ScrollableThresholdValueLabel.Text = $"Scrollable threshold: {_options.ScrollableThreshold}";
        }
    }

    private sealed record TabStyleOption(string Label, TabVisualStyle Style);
    private sealed record TabLayoutModeOption(string Label, TabLayoutMode Mode);
    private sealed record ReselectBehaviorOption(string Label, TabReselectBehavior Behavior);
}
