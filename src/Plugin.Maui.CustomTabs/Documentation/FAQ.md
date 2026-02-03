# FAQ

## Does this replace Shell or TabbedPage?
Yes. The library renders a fully custom, UI-driven tab bar and swaps the active NavigationPage per tab, so you can skip Shell or TabbedPage entirely.

## How does Android back work?
The host page checks the current tab's navigation stack. If there are pages to pop, it pops. Otherwise, it defers to the default behavior (exit or modal pop).

## How is iOS safe area handled?
When `RespectSafeArea` is enabled, the host page respects iOS safe area insets so the bar does not overlap the home indicator.

## Can I use image icons instead of glyphs?
Yes. Supply `IconProvider` on `CustomTabItem`. If it is set, the bar renders the image source instead of the glyph.

## Why are my glyphs not showing?
Provide a `FontFamily` in `CustomTabsOptions` if you use a custom icon font. Without it, the platform default font is used.

## How do I select a tab programmatically?
Bind to `SelectedTabKey` or `SelectedIndex` on `CustomTabsHostPage` (two-way), or call `SelectTabByKey`/`SelectTabByIndex` on the view model.

## How do I block tab selection (auth gating)?
Subscribe to `TabSelectionRequested` and set `Cancel = true`, or provide a `CanSelectTab` predicate.

## How do I handle reselect (tap active tab)?
Set `CustomTabsOptions.ReselectBehavior` (e.g., `PopToRoot`, `ScrollToTop`, `Command`). To support scroll-to-top, implement `ITabScrollToTop`.

## How do I refresh an icon at runtime?
Use `IconSource` for static icons or `IconProvider` for dynamic ones, then call `NotifyIconChanged()` on the tab item.

## How do I make the tab bar scrollable?
Set `TabLayoutMode` to `Scrollable`, or keep `Auto` and adjust `ScrollableThreshold`.
