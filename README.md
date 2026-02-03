# Plugin.Maui.CustomTabs

Fully customizable, brand-driven bottom navigation for .NET MAUI (iOS/Android) with per-tab navigation stacks, localization-ready titles, optional text, badges, and animations.

## Why not TabbedPage or Shell?
`TabbedPage` and Shell's tab bar provide limited layout control and styling. This library replaces the tab bar with a UI-driven control so you can:
- Align the tab bar with brand design systems
- Animate and style the underline and icons freely
- Keep independent navigation stacks per tab
- Toggle text visibility and update options at runtime

## Installation

Add the package and register resources:

```bash
dotnet add package Plugin.Maui.CustomTabs
```

```csharp
using Plugin.Maui.CustomTabs.Extensions;

builder.UseCustomTabs();
```

## Quick start

```csharp
using Plugin.Maui.CustomTabs.Extensions;
using Plugin.Maui.CustomTabs.Models;

var options = new CustomTabsOptions
{
    BackgroundColor = Color.FromArgb("#071A3A"),
    AccentColor = Color.FromArgb("#D4AF37")
};

var tabs = new List<CustomTabItem>
{
    new CustomTabItem("home", "H", () => new HomePage()) { Title = "Home" },
    new CustomTabItem("search", "S", () => new SearchPage()) { Title = "Search" },
    new CustomTabItem("settings", "T", () => new SettingsPage()) { Title = "Settings" }
};

MainPage = CustomTabs.Create(tabs, "home", options);
```

## App setup options

You can initialize the custom tabs page in two common ways:

### 1) Plain MAUI (no navigation framework)

```csharp
public partial class App : Application
{
    private readonly Page _rootPage;

    public App()
    {
        InitializeComponent();

        var options = new CustomTabsOptions();
        var tabs = new List<CustomTabItem>
        {
            new CustomTabItem("home", "H", () => new HomePage()) { Title = "Home" }
        };

        _rootPage = CustomTabs.Create(tabs, "home", options);
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(_rootPage);
}
```

#### Plain MAUI splash + auth routing (no Prism)

If you have a splash page that decides where to go, swap the root page after your login check:

```csharp
protected override async void OnAppearing()
{
    base.OnAppearing();

    var isLoggedIn = await authService.IsLoggedInAsync();
    var root = isLoggedIn
        ? CustomTabs.Create(tabs, "home", options, localizationService)
        : new NavigationPage(new LoginPage());

    Application.Current!.Windows[0].Page = root;
}
```

### 2) Prism (splash + auth routing)

Create a Prism-friendly tabs page that derives from `CustomTabsHostPage`, then navigate to it after your splash check.

```csharp
public sealed class MainTabsPage : CustomTabsHostPage
{
    public MainTabsPage(SimpleLocalizationService localization)
        : base(CreateViewModel(localization), localization)
    {
    }

    private static CustomTabsViewModel CreateViewModel(SimpleLocalizationService localization)
    {
        var options = new CustomTabsOptions();
        var tabs = new List<CustomTabItem>
        {
            new CustomTabItem("home", "H", () => new HomePage())
            { TitleProvider = () => localization.Translate("Home") }
        };

        return new CustomTabsViewModel(tabs, "home", options);
    }
}
```

```csharp
prism.RegisterForNavigation<SplashPage, SplashPageViewModel>();
prism.RegisterForNavigation<LoginPage, LoginPageViewModel>();
prism.RegisterForNavigation<MainTabsPage>();
```

```csharp
var target = isLoggedIn ? "/MainTabsPage" : "/NavigationPage/LoginPage";
await NavigationService.NavigateAsync(target);
```

## Options

| Property | Type | Default | Notes |
| --- | --- | --- | --- |
| `ShowText` | `bool` | `true` | Toggle text labels at runtime. |
| `TabBarHeight` | `double` | `76` | Overall tab bar height. |
| `BackgroundColor` | `Color` | `#071A3A` | Tab bar background. |
| `AccentColor` | `Color` | `#D4AF37` | Underline + selected tint. |
| `SelectedTextColor` | `Color` | `White` | Selected label color. |
| `UnselectedTextColor` | `Color` | `LightGray` | Unselected label color. |
| `SelectedIconColor` | `Color` | `#D4AF37` | Selected glyph color. |
| `UnselectedIconColor` | `Color` | `LightGray` | Unselected glyph color. |
| `EnableHaptics` | `bool` | `false` | Light haptic feedback on selection. |
| `EnableAnimations` | `bool` | `true` | Enables icon + underline animations. |
| `AnimationDuration` | `TimeSpan` | `160ms` | Duration for selection animations. |
| `FontFamily` | `string?` | `null` | Optional icon/text font family. |
| `RespectSafeArea` | `bool` | `true` on iOS | Keeps the bar above safe area. |
| `IconSize` | `double` | `26` | Icon size in device units. |
| `TextSize` | `double` | `12` | Label size in device units. |
| `TabBarPadding` | `Thickness` | `0,10,0,16` | Inner padding of the tab bar. |
| `TabBarOuterMargin` | `Thickness` | `0` | Outer margin for floating tab bars. |
| `ShowUnderline` | `bool` | `true` | Toggle the underline indicator. |
| `UnderlineHeight` | `double` | `2` | Height of the underline. |
| `UnderlineMargin` | `Thickness` | `0,6,0,0` | Spacing between text and underline. |
| `BadgeOffsetX` | `double` | `6` | Horizontal badge offset from the icon. |
| `BadgeOffsetY` | `double` | `-4` | Vertical badge offset from the icon. |

All options are mutable at runtime. Any changes trigger UI updates.

## Localization

### Option 1: TitleProvider
Provide a function that returns the current localized string and call `RefreshTitles()` when your language changes.

```csharp
var tab = new CustomTabItem("home", "H", () => new HomePage())
{
    TitleProvider = () => localizer["Home"]
};

viewModel.RefreshTitles();
```

### Option 2: ILocalizationRefreshService
Implement `ILocalizationRefreshService` and raise `LanguageChanged`. The host page automatically calls `RefreshTitles()`.

```csharp
public sealed class LocalizationService : ILocalizationRefreshService
{
    public event EventHandler? LanguageChanged;

    public void SetLanguage(string language)
    {
        // Update your culture then:
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

## Badges

```csharp
var badge = new CustomTabBadge { Count = 3 };
var tab = new CustomTabItem("messages", "M", () => new MessagesPage())
{
    Title = "Messages",
    Badge = badge
};

badge.Count += 1; // updates instantly
```

## Animations and haptics

```csharp
options.EnableAnimations = true;
options.EnableHaptics = true;
options.AnimationDuration = TimeSpan.FromMilliseconds(160);
```

## Safe area behavior
When `RespectSafeArea` is enabled, the host page ensures the bar does not overlap the iOS home indicator. On Android, the bar relies on system insets and includes extra bottom padding by default.

## FAQ
See `src/Plugin.Maui.CustomTabs/Documentation/FAQ.md` for more details.
