using System;
using System.Collections.Generic;
using CustomTabs.Sample.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Plugin.Maui.CustomTabs.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.ComponentModel;
using System.Linq;

namespace CustomTabs.Sample.ViewModels;

/// <summary>
/// View model for the settings page.
/// </summary>
public sealed class SettingsPageViewModel : BindableBase, IDestructible
{
    private readonly SimpleLocalizationService _localization;
    private readonly INavigationService _navigationService;
    private bool _suppressSelectionSync;
    private TabStyleOption? _selectedStyle;
    private TabLayoutModeOption? _selectedLayoutMode;
    private ReselectBehaviorOption? _selectedReselectBehavior;

    /// <summary>
    /// Options applied to the tab bar.
    /// </summary>
    public CustomTabsOptions Options { get; }

    /// <summary>
    /// Supported visual styles.
    /// </summary>
    public IReadOnlyList<TabStyleOption> StyleOptions { get; }

    /// <summary>
    /// Supported layout modes.
    /// </summary>
    public IReadOnlyList<TabLayoutModeOption> LayoutModes { get; }

    /// <summary>
    /// Supported reselect behaviors.
    /// </summary>
    public IReadOnlyList<ReselectBehaviorOption> ReselectBehaviors { get; }

    /// <summary>
    /// Selected visual style.
    /// </summary>
    public TabStyleOption? SelectedStyle
    {
        get => _selectedStyle;
        set
        {
            if (SetProperty(ref _selectedStyle, value) && value != null && !_suppressSelectionSync)
            {
                Options.VisualStyle = value.Style;
            }
        }
    }

    /// <summary>
    /// Selected layout mode.
    /// </summary>
    public TabLayoutModeOption? SelectedLayoutMode
    {
        get => _selectedLayoutMode;
        set
        {
            if (SetProperty(ref _selectedLayoutMode, value) && value != null && !_suppressSelectionSync)
            {
                Options.TabLayoutMode = value.Mode;
            }
        }
    }

    /// <summary>
    /// Selected reselect behavior.
    /// </summary>
    public ReselectBehaviorOption? SelectedReselectBehavior
    {
        get => _selectedReselectBehavior;
        set
        {
            if (SetProperty(ref _selectedReselectBehavior, value) && value != null && !_suppressSelectionSync)
            {
                Options.ReselectBehavior = value.Behavior;
            }
        }
    }

    /// <summary>
    /// Slider value for tab bar corner radius.
    /// </summary>
    public double CornerRadiusValue
    {
        get => Options.TabBarCornerRadius.TopLeft;
        set
        {
            if (Math.Abs(Options.TabBarCornerRadius.TopLeft - value) < 0.01)
            {
                return;
            }

            Options.TabBarCornerRadius = new CornerRadius(value);
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(CornerRadiusLabel));
        }
    }

    /// <summary>
    /// Stepper value for the scrollable threshold.
    /// </summary>
    public double ScrollableThresholdValue
    {
        get => Options.ScrollableThreshold;
        set
        {
            var rounded = (int)Math.Round(value);
            if (Options.ScrollableThreshold == rounded)
            {
                return;
            }

            Options.ScrollableThreshold = rounded;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(ScrollableThresholdLabel));
        }
    }

    /// <summary>
    /// Label for icon size slider.
    /// </summary>
    public string IconSizeLabel => $"Icon size: {Options.IconSize:F0}";

    /// <summary>
    /// Label for text size slider.
    /// </summary>
    public string TextSizeLabel => $"Text size: {Options.TextSize:F0}";

    /// <summary>
    /// Label for tab bar height slider.
    /// </summary>
    public string TabBarHeightLabel => $"Tab bar height: {Options.TabBarHeight:F0}";

    /// <summary>
    /// Label for border thickness slider.
    /// </summary>
    public string BorderThicknessLabel => $"Border thickness: {Options.BorderThickness:F1}";

    /// <summary>
    /// Label for corner radius slider.
    /// </summary>
    public string CornerRadiusLabel => $"Tab bar corner radius: {Options.TabBarCornerRadius.TopLeft:F0}";

    /// <summary>
    /// Label for scrollable threshold stepper.
    /// </summary>
    public string ScrollableThresholdLabel => $"Scrollable threshold: {Options.ScrollableThreshold}";

    /// <summary>
    /// Label for current language.
    /// </summary>
    public string CurrentLanguageLabel => $"Current: {_localization.CurrentLanguage.ToUpperInvariant()}";

    /// <summary>
    /// Command to apply the neutral theme.
    /// </summary>
    public DelegateCommand ApplyNeutralThemeCommand { get; }

    /// <summary>
    /// Command to apply the slate theme.
    /// </summary>
    public DelegateCommand ApplySlateThemeCommand { get; }

    /// <summary>
    /// Command to apply the muted theme.
    /// </summary>
    public DelegateCommand ApplyMutedThemeCommand { get; }

    /// <summary>
    /// Command to set English language.
    /// </summary>
    public DelegateCommand SetEnglishCommand { get; }

    /// <summary>
    /// Command to set French language.
    /// </summary>
    public DelegateCommand SetFrenchCommand { get; }

    /// <summary>
    /// Command to push details.
    /// </summary>
    public DelegateCommand PushDetailsCommand { get; }


    /// <summary>
    /// Creates the settings view model.
    /// </summary>
    public SettingsPageViewModel(CustomTabsOptions options,
        SimpleLocalizationService localization,
        INavigationService navigationService)
    {
        Options = options ?? new CustomTabsOptions();
        _localization = localization ?? new SimpleLocalizationService();
        _navigationService = navigationService;

        StyleOptions = new List<TabStyleOption>
        {
            new("1. Classic Bottom", TabVisualStyle.ClassicBottom),
            new("2. Top Underline", TabVisualStyle.TopUnderline),
            new("3. Segmented", TabVisualStyle.Segmented),
            new("4. Floating", TabVisualStyle.Floating),
            new("6. Icon Only", TabVisualStyle.IconOnly),
            new("8. Compact", TabVisualStyle.Compact),
            new("9. Minimal", TabVisualStyle.Minimal),
            new("10. Pills", TabVisualStyle.Pills)
        };

        LayoutModes = new List<TabLayoutModeOption>
        {
            new("Layout: Fixed", TabLayoutMode.Fixed),
            new("Layout: Scrollable", TabLayoutMode.Scrollable),
            new("Layout: Auto", TabLayoutMode.Auto)
        };

        ReselectBehaviors = new List<ReselectBehaviorOption>
        {
            new("Reselect: None", TabReselectBehavior.None),
            new("Reselect: Pop To Root", TabReselectBehavior.PopToRoot),
            new("Reselect: Pop + Scroll Top", TabReselectBehavior.PopToRoot | TabReselectBehavior.ScrollToTop),
            new("Reselect: Command", TabReselectBehavior.Command)
        };

        ApplyNeutralThemeCommand = new DelegateCommand(() =>
            ApplyThemePreset("#1F2937", "#E5E7EB", "#F9FAFB", "#9CA3AF", "#374151"));
        ApplySlateThemeCommand = new DelegateCommand(() =>
            ApplyThemePreset("#0F172A", "#CBD5E1", "#E2E8F0", "#94A3B8", "#334155"));
        ApplyMutedThemeCommand = new DelegateCommand(() =>
            ApplyThemePreset("#111827", "#D1D5DB", "#F3F4F6", "#9CA3AF", "#4B5563"));

        SetEnglishCommand = new DelegateCommand(() =>
            SafeExecution.Run(() => _localization.SetLanguage("en"), "SettingsPageViewModel.SetEnglishCommand"));
        SetFrenchCommand = new DelegateCommand(() =>
            SafeExecution.Run(() => _localization.SetLanguage("fr"), "SettingsPageViewModel.SetFrenchCommand"));

        PushDetailsCommand = new DelegateCommand(async () =>
            await SafeExecution.RunAsync(
                () => NavigateToDetailAsync("Settings details"),
                "SettingsPageViewModel.PushDetailsCommand"));


        SyncFromOptions();
        Options.PropertyChanged += OnOptionsPropertyChanged;
        _localization.LanguageChanged += OnLanguageChanged;
    }

    private void ApplyThemePreset(string background, string accent, string selected, string unselected, string border)
    {
        SafeExecution.Run(() =>
        {
            Options.BackgroundColor = Color.FromArgb(background);
            Options.AccentColor = Color.FromArgb(accent);
            Options.SelectedIconColor = Color.FromArgb(selected);
            Options.UnselectedIconColor = Color.FromArgb(unselected);
            Options.SelectedTextColor = Color.FromArgb(selected);
            Options.UnselectedTextColor = Color.FromArgb(unselected);
            Options.BorderColor = Color.FromArgb(border);
        }, "SettingsPageViewModel.ApplyThemePreset");
    }

    private Task NavigateToDetailAsync(string title)
    {
        var parameters = new NavigationParameters
        {
            { "title", title }
        };

        return _navigationService.NavigateAsync("DetailPage", parameters);
    }


    private void SyncFromOptions()
    {
        _suppressSelectionSync = true;
        try
        {
            SelectedStyle = StyleOptions.FirstOrDefault(item => item.Style == Options.VisualStyle);
            SelectedLayoutMode = LayoutModes.FirstOrDefault(item => item.Mode == Options.TabLayoutMode);
            SelectedReselectBehavior = ReselectBehaviors.FirstOrDefault(item => item.Behavior == Options.ReselectBehavior);
        }
        finally
        {
            _suppressSelectionSync = false;
        }
    }

    private void OnOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CustomTabsOptions.IconSize):
                RaisePropertyChanged(nameof(IconSizeLabel));
                break;
            case nameof(CustomTabsOptions.TextSize):
                RaisePropertyChanged(nameof(TextSizeLabel));
                break;
            case nameof(CustomTabsOptions.TabBarHeight):
                RaisePropertyChanged(nameof(TabBarHeightLabel));
                break;
            case nameof(CustomTabsOptions.BorderThickness):
                RaisePropertyChanged(nameof(BorderThicknessLabel));
                break;
            case nameof(CustomTabsOptions.TabBarCornerRadius):
                RaisePropertyChanged(nameof(CornerRadiusValue));
                RaisePropertyChanged(nameof(CornerRadiusLabel));
                break;
            case nameof(CustomTabsOptions.ScrollableThreshold):
                RaisePropertyChanged(nameof(ScrollableThresholdValue));
                RaisePropertyChanged(nameof(ScrollableThresholdLabel));
                break;
            case nameof(CustomTabsOptions.VisualStyle):
            case nameof(CustomTabsOptions.TabLayoutMode):
            case nameof(CustomTabsOptions.ReselectBehavior):
                SyncFromOptions();
                break;
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(CurrentLanguageLabel));
    }

    /// <inheritdoc />
    public void Destroy()
    {
        Options.PropertyChanged -= OnOptionsPropertyChanged;
        _localization.LanguageChanged -= OnLanguageChanged;
    }

    public sealed record TabStyleOption(string Label, TabVisualStyle Style);
    public sealed record TabLayoutModeOption(string Label, TabLayoutMode Mode);
    public sealed record ReselectBehaviorOption(string Label, TabReselectBehavior Behavior);
}
