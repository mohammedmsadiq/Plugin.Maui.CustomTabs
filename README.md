# Plugin.Maui.CustomTabs

Fully customizable tab navigation for .NET MAUI (iOS/Android) with per-tab navigation stacks, localization-ready titles, optional text, badges, and animations.

## Why not TabbedPage or Shell?
`TabbedPage` and Shell's tab bar provide limited layout control and styling. This library replaces the tab bar with a UI-driven control so you can:
- Align the tab bar with your own design system
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

## Starter guide (no Prism)

This is the simplest way to get custom tabs working in a plain MAUI app.

1. Install the package and call `UseCustomTabs()`.
2. Create your tabs and options.
3. Set the root page to `CustomTabs.Create(...)`.

```csharp
using Plugin.Maui.CustomTabs.Extensions;
using Plugin.Maui.CustomTabs.Models;

// MauiProgram.cs
builder.UseCustomTabs();

// App.xaml.cs
public partial class App : Application
{
    private readonly Page _rootPage;

    public App()
    {
        InitializeComponent();

        var options = new CustomTabsOptions
        {
            BackgroundColor = Colors.White,
            AccentColor = Colors.Gray,
            SelectedTextColor = Colors.Black,
            UnselectedTextColor = Colors.Gray,
            SelectedIconColor = Colors.Black,
            UnselectedIconColor = Colors.Gray
        };

        var tabs = new List<CustomTabItem>
        {
            new CustomTabItem("home", "H", () => new HomePage()) { Title = "Home" },
            new CustomTabItem("search", "S", () => new SearchPage()) { Title = "Search" },
            new CustomTabItem("settings", "G", () => new SettingsPage()) { Title = "Settings" }
        };

        _rootPage = CustomTabs.Create(tabs, "home", options);
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(_rootPage);
}
```

## Starter guide (with Prism)

If your app uses Prism, you can keep full DI + navigation by resolving tab pages from the Prism container. The sample uses the small helper project below.

Sample: `sample/CustomTabs.Prism.Sample`

1. Add a reference to `src/Plugin.Maui.CustomTabs.Prism/Plugin.Maui.CustomTabs.Prism.csproj`.
2. Register all pages for navigation.
3. Build tabs via `PrismCustomTabs.CreateTab(...)` so each tab root is container-resolved and hosted in a `PrismNavigationPage`.
4. Navigate to the tabs page using Prism navigation.

```csharp
using Plugin.Maui.CustomTabs.Models;
using Plugin.Maui.CustomTabs.Pages;
using Plugin.Maui.CustomTabs.Prism;
using Plugin.Maui.CustomTabs.ViewModels;
using Prism.Ioc;

public sealed class MainTabsPage : CustomTabsHostPage
{
    public MainTabsPage(IContainerProvider container, SimpleLocalizationService localization, CustomTabsOptions options)
        : base(CreateViewModel(container, localization, options), localization)
    {
        Title = "Custom Tabs";
        NavigationPage.SetHasNavigationBar(this, true);
        NavigationPage.SetTitleView(this, new Label { Text = "Custom Tabs" });
    }

    private static CustomTabsViewModel CreateViewModel(
        IContainerProvider container,
        SimpleLocalizationService localization,
        CustomTabsOptions options)
    {
        var tabs = new List<CustomTabItem>
        {
            PrismCustomTabs.CreateTab<HomePage>(container, "home", "H", tab =>
            {
                tab.TitleProvider = () => localization.Translate("Home");
            }),
            PrismCustomTabs.CreateTab<SearchPage>(container, "search", "S", tab =>
            {
                tab.TitleProvider = () => localization.Translate("Search");
            }),
            PrismCustomTabs.CreateTab<SettingsPage>(container, "settings", "G", tab =>
            {
                tab.TitleProvider = () => localization.Translate("Settings");
            })
        };

        return PrismCustomTabs.CreateViewModel(tabs, "home", options);
    }
}
```

```csharp
// Prism registration
prism.RegisterForNavigation<MainTabsPage>();
prism.RegisterForNavigation<HomePage, HomePageViewModel>();
prism.RegisterForNavigation<SearchPage, SearchPageViewModel>();
prism.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();

// Navigate after splash/auth (wrap in NavigationPage to show nav bar / TitleView)
await NavigationService.NavigateAsync("/NavigationPage/MainTabsPage");
```

### Tab selection (Prism)

Prism exposes `SelectTabAsync`. The sample wires this in the Home page to demonstrate tab switching:

```csharp
await _navigationService.SelectTabAsync("messages");
```

If `SelectTabAsync` can’t resolve a tab (because custom tabs are not a `TabbedPage`), you can fall back to `CustomTabsHostPage.SelectedTabKey` directly.

## Safe area + layout notes

- The tab bar now fills the bottom edge of the screen, while its internal padding keeps icons above the iOS home indicator.
- Use `PageBackgroundColor` and `ContentBackgroundColor` to separate the tab bar, page, and content colors.

## Options

| Property | Type | Default | Notes |
| --- | --- | --- | --- |
| `ShowText` | `bool` | `true` | Toggle text labels at runtime. |
| `TabBarHeight` | `double` | `76` | Overall tab bar height. |
| `BackgroundColor` | `Color` | `#1F2937` | Tab bar background. |
| `PageBackgroundColor` | `Color?` | `null` | Optional host page background. Defaults to `BackgroundColor`. |
| `ContentBackgroundColor` | `Color?` | `null` | Optional content host background. Defaults to `PageBackgroundColor`. |
| `AccentColor` | `Color` | `#9CA3AF` | Underline + selected tint. |
| `SelectedTextColor` | `Color` | `#F9FAFB` | Selected label color. |
| `UnselectedTextColor` | `Color` | `#9CA3AF` | Unselected label color. |
| `SelectedIconColor` | `Color` | `#F9FAFB` | Selected glyph color. |
| `UnselectedIconColor` | `Color` | `#9CA3AF` | Unselected glyph color. |
| `EnableHaptics` | `bool` | `false` | Light haptic feedback on selection. |
| `EnableAnimations` | `bool` | `true` | Enables icon + underline animations. |
| `AnimationDuration` | `TimeSpan` | `160ms` | Duration for selection animations. |
| `IconAnimationStyle` | `TabSelectionAnimationStyle` | `Scale` | Icon animation style (`None`, `Scale`, `Fade`). |
| `IndicatorAnimationStyle` | `TabSelectionAnimationStyle` | `Scale` | Indicator animation style. |
| `FontFamily` | `string?` | `null` | Optional icon/text font family. |
| `RespectSafeArea` | `bool` | `true` on iOS | Keeps the bar above safe area. |
| `RespectBottomSafeArea` | `bool` | `true` | Applies the bottom safe area inset when `RespectSafeArea` is enabled. |
| `IconSize` | `double` | `26` | Icon size in device units. |
| `TextSize` | `double` | `12` | Label size in device units. |
| `TabBarPadding` | `Thickness` | `0,10,0,16` | Inner padding of the tab bar. |
| `TabBarOuterMargin` | `Thickness` | `0` | Outer margin for floating tab bars. |
| `TabBarCornerRadius` | `CornerRadius` | `0` | Rounded corners for the tab bar container. |
| `BorderColor` | `Color` | `Transparent` | Tab bar border color. |
| `BorderThickness` | `double` | `0` | Tab bar border thickness. |
| `TabBarShadow` | `Shadow?` | `null` | Optional shadow for the tab bar. |
| `TabLayoutMode` | `TabLayoutMode` | `Auto` | `Fixed`, `Scrollable`, or `Auto` (uses `ScrollableThreshold`). |
| `VisualStyle` | `TabVisualStyle` | `ClassicBottom` | Preset tab UI style (supports options 1,2,3,4,6,8,9,10). |
| `ScrollableThreshold` | `int` | `5` | Tab count that triggers auto scroll. |
| `TabItemWidth` | `double` | `-1` | Fixed width for tab items (`-1` for auto). |
| `TabItemMinWidth` | `double` | `0` | Minimum width for tab items. |
| `TabItemTemplate` | `DataTemplate?` | `null` | Custom tab item template. |
| `ShowUnderline` | `bool` | `true` | Toggle the underline indicator. |
| `UnderlineHeight` | `double` | `2` | Height of the underline. |
| `UnderlineMargin` | `Thickness` | `0,6,0,0` | Spacing between text and underline. |
| `IndicatorWidth` | `double` | `-1` | Fixed indicator width (`-1` for auto). |
| `IndicatorHorizontalOptions` | `LayoutOptions` | `Fill` | Horizontal alignment for the indicator. |
| `IndicatorCornerRadius` | `float` | `0` | Corner radius for the indicator. |
| `BadgeOffsetX` | `double` | `6` | Horizontal badge offset from the icon. |
| `BadgeOffsetY` | `double` | `-4` | Vertical badge offset from the icon. |
| `ReselectBehavior` | `TabReselectBehavior` | `None` | Reselect behavior for active tab taps. |
| `EnableKeyboardNavigation` | `bool` | `true` | Enables arrow-key navigation on desktop. |
| `NavigationPageFactory` | `Func<Page, NavigationPage>?` | `null` | Custom navigation page factory. |
| `NavigationBarVisible` | `bool?` | `null` | Default nav bar visibility. |
| `NavigationBarBackgroundColor` | `Color?` | `null` | Default nav bar background color. |
| `NavigationBarTextColor` | `Color?` | `null` | Default nav bar text color. |

All options are mutable at runtime. Any changes trigger UI updates.

## Visual style presets

`options.VisualStyle` supports these implemented presets:

- `1` `ClassicBottom`
- `2` `TopUnderline`
- `3` `Segmented`
- `4` `Floating`
- `6` `IconOnly`
- `8` `Compact`
- `9` `Minimal`
- `10` `Pills`

Example:

```csharp
options.VisualStyle = TabVisualStyle.Pills;
```

## Selection and gating

The host page exposes two-way bindable selection properties:

- `SelectedTabKey`
- `SelectedIndex`

You can observe or gate selection with:

- `SelectedTabChanged`
- `TabSelectionRequested` (set `Cancel = true` to block)
- `CanSelectTab` predicate

```csharp
var page = (CustomTabsHostPage)CustomTabs.Create(tabs, "home", options);
page.TabSelectionRequested += (_, args) =>
{
    if (args.Tab.Key == "settings" && !isLoggedIn)
    {
        args.Cancel = true;
    }
};

page.SelectedTabKey = "search";
```

## Reselect behavior

Enable one or more behaviors when the active tab is tapped again:

```csharp
options.ReselectBehavior = TabReselectBehavior.PopToRoot | TabReselectBehavior.ScrollToTop;

tabs[0].ReselectCommand = new Command(() => logger.Log("Home reselected"));
```

To support `ScrollToTop`, implement `ITabScrollToTop` on the current page or its binding context.

## Custom templates

Set `options.TabItemTemplate` to fully customize the tab item UI. Your template should include a tap gesture that calls `SelectTabCommand` and passes the tab item as the parameter. If you want built-in animations, name the elements `IconContainer` and `Underline`.

## Per-tab customization

`CustomTabItem` supports per-tab overrides:

- `IsEnabled`, `IsVisible`
- `IconSource` or `IconProvider`
- `SelectedTextColor`, `UnselectedTextColor`
- `SelectedIconColor`, `UnselectedIconColor`
- `IndicatorColor`
- `NavigationPageFactory`, `NavigationBarVisible`, `NavigationBarBackgroundColor`, `NavigationBarTextColor`
- `AutomationId`, `AutomationName`, `AutomationDescription`

## Icon refresh

Use `IconSource` for static images, or `IconProvider` for dynamic ones. Call `NotifyIconChanged()` to refresh icon bindings at runtime.

## Badge customization

`CustomTabBadge` now supports:

- `Text`, `MaxCount`
- `FontSize`, `MinWidth`, `MinHeight`, `CornerRadius`
- `BorderColor`, `BorderThickness`
- `DotSize`

## Accessibility

Set `AutomationId`, `AutomationName`, and `AutomationDescription` on each `CustomTabItem` to improve testing and screen reader output.

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
var badge = new CustomTabBadge
{
    Count = 3,
    MaxCount = 99,
    BorderColor = Colors.White,
    BorderThickness = 1
};
var tab = new CustomTabItem("messages", "M", () => new MessagesPage())
{
    Title = "Messages",
    Badge = badge
};

badge.Count += 1; // updates instantly
badge.Text = "New"; // overrides count display
```

## Animations and haptics

```csharp
options.EnableAnimations = true;
options.EnableHaptics = true;
options.AnimationDuration = TimeSpan.FromMilliseconds(160);
options.IconAnimationStyle = TabSelectionAnimationStyle.Scale;
options.IndicatorAnimationStyle = TabSelectionAnimationStyle.Fade;
```

## Safe area behavior
When `RespectSafeArea` is enabled, the host page ensures the bar does not overlap the iOS home indicator. On Android, it applies window inset padding (including gesture navigation and cutouts) to keep the bar visible in edge cases.

## Testing

- Unit tests live in `tests/Plugin.Maui.CustomTabs.Tests`.
- By default the test project targets `net9.0` only. To include MAUI targets, run:
  `dotnet test tests/Plugin.Maui.CustomTabs.Tests/Plugin.Maui.CustomTabs.Tests.csproj -c Release -p:MAUI_TESTS=true`
- UI tests live in `tests/Plugin.Maui.CustomTabs.UITests` and use Appium. Set `MAUI_APP_PATH`, `MAUI_PLATFORM`, and (optionally) `APPIUM_SERVER_URL`, `APPIUM_DEVICE_NAME`.

## FAQ
See `src/Plugin.Maui.CustomTabs/Documentation/FAQ.md` for more details.
