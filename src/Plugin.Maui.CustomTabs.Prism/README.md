# Plugin.Maui.CustomTabs.Prism

Helper utilities for integrating Plugin.Maui.CustomTabs with Prism.Maui. This project provides `PrismCustomTabs` to resolve tab root pages from the Prism container and host each tab in a `PrismNavigationPage`.

## Usage

1. Reference this project or package.
2. Register your pages for navigation in Prism.
3. Build tabs via `PrismCustomTabs.CreateTab`.

```csharp
var tabs = new List<CustomTabItem>
{
    PrismCustomTabs.CreateTab<HomePage>(container, "home", "H"),
    PrismCustomTabs.CreateTab<SearchPage>(container, "search", "S")
};

var viewModel = PrismCustomTabs.CreateViewModel(tabs, "home", options);
```

Use Prism navigation for tab switching where appropriate:

```csharp
await _navigationService.SelectTabAsync("search");
```

If the tab cannot be resolved via Prism (custom tabs are not a TabbedPage), fall back to setting `CustomTabsHostPage.SelectedTabKey` directly.
