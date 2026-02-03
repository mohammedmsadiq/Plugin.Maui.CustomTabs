using Plugin.Maui.CustomTabs.Services;

namespace CustomTabs.Sample.Services;

/// <summary>
/// Simple in-memory localization service for demo purposes.
/// </summary>
public sealed class SimpleLocalizationService : ILocalizationRefreshService
{
    private readonly Dictionary<string, Dictionary<string, string>> _translations =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["en"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Home"] = "Home",
                ["Search"] = "Search",
                ["Messages"] = "Messages",
                ["Profile"] = "Profile",
                ["Settings"] = "Settings"
            },
            ["fr"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Home"] = "Accueil",
                ["Search"] = "Recherche",
                ["Messages"] = "Messages",
                ["Profile"] = "Profil",
                ["Settings"] = "Parametres"
            }
        };

    /// <inheritdoc />
    public event EventHandler? LanguageChanged;

    /// <summary>
    /// Current language code.
    /// </summary>
    public string CurrentLanguage { get; private set; } = "en";

    /// <summary>
    /// Translates the given key using the current language.
    /// </summary>
    public string Translate(string key)
    {
        if (_translations.TryGetValue(CurrentLanguage, out var map) && map.TryGetValue(key, out var value))
        {
            return value;
        }

        return key;
    }

    /// <summary>
    /// Sets the current language and notifies listeners.
    /// </summary>
    public void SetLanguage(string language)
    {
        if (string.Equals(CurrentLanguage, language, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!_translations.ContainsKey(language))
        {
            return;
        }

        CurrentLanguage = language;
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}
