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
