using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Globalization;
using System.Collections.Generic;
using Windows.Globalization;

namespace MessagesWinUI.Helpers;

/// <summary>
/// Helper class for handling localization and resource strings using proper WinUI 3 APIs
/// </summary>
public static class LocalizationHelper
{
    private static ResourceLoader? _resourceLoader;
    private static string _currentLanguage = "en";

    /// <summary>
    /// Initialize the resource loader
    /// </summary>
    public static void Initialize()
    {
        try
        {
            _currentLanguage = GetCurrentLanguageCode();
            
            // Set app language to match system if supported
            if (_currentLanguage == "es")
            {
                ApplicationLanguages.PrimaryLanguageOverride = "es";
                System.Diagnostics.Debug.WriteLine("System language detected as Spanish, setting app to Spanish");
            }
            else
            {
                ApplicationLanguages.PrimaryLanguageOverride = "en";
                System.Diagnostics.Debug.WriteLine("System language detected as English or other, setting app to English");
            }
            
            _resourceLoader = new ResourceLoader();
            System.Diagnostics.Debug.WriteLine($"ResourceLoader initialized. System UI Culture: {CultureInfo.CurrentUICulture.Name}, App Language: {ApplicationLanguages.PrimaryLanguageOverride}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to initialize ResourceLoader: {ex.Message}");
        }
    }

    /// <summary>
    /// Initialize the resource loader with specific language
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "es", "en")</param>
    public static void Initialize(string languageCode)
    {
        try
        {
            // Set the primary language override for WinUI 3
            ApplicationLanguages.PrimaryLanguageOverride = languageCode;
            
            // Recreate ResourceLoader to use new language
            _resourceLoader = new ResourceLoader();
            _currentLanguage = languageCode;
            
            System.Diagnostics.Debug.WriteLine($"Language changed to: {languageCode}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to set language to {languageCode}: {ex.Message}");
            // Fallback to default
            Initialize();
        }
    }

    /// <summary>
    /// Get a localized string by key
    /// </summary>
    /// <param name="key">Resource key</param>
    /// <returns>Localized string or key if not found</returns>
    public static string GetString(string key)
    {
        try
        {
            if (_resourceLoader == null)
            {
                Initialize();
            }

            var result = _resourceLoader?.GetString(key);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            return key; // Return key itself as last resort
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to get resource string for key '{key}': {ex.Message}");
            return key;
        }
    }

    /// <summary>
    /// Get a localized string with format parameters
    /// </summary>
    /// <param name="key">Resource key</param>
    /// <param name="args">Format arguments</param>
    /// <returns>Formatted localized string</returns>
    public static string GetString(string key, params object[] args)
    {
        try
        {
            var format = GetString(key);
            return string.Format(format, args);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to format resource string for key '{key}': {ex.Message}");
            return key;
        }
    }

    /// <summary>
    /// Get the current system language code
    /// </summary>
    /// <returns>Language code (e.g., "en", "es")</returns>
    public static string GetCurrentLanguageCode()
    {
        try
        {
            var culture = CultureInfo.CurrentUICulture;
            return culture.TwoLetterISOLanguageName;
        }
        catch
        {
            return "en"; // Default to English
        }
    }

    /// <summary>
    /// Get the current language name
    /// </summary>
    /// <returns>Language name (e.g., "English", "Español")</returns>
    public static string GetCurrentLanguageName()
    {
        try
        {
            var culture = CultureInfo.CurrentUICulture;
            return culture.DisplayName;
        }
        catch
        {
            return "English";
        }
    }

    /// <summary>
    /// Check if the current language is Spanish
    /// </summary>
    /// <returns>True if current language is Spanish</returns>
    public static bool IsSpanish()
    {
        return GetCurrentLanguageCode().Equals("es", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Check if the current language is English
    /// </summary>
    /// <returns>True if current language is English</returns>
    public static bool IsEnglish()
    {
        return GetCurrentLanguageCode().Equals("en", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Force switch to Spanish for testing
    /// </summary>
    public static void SwitchToSpanish()
    {
        Initialize("es");
    }

    /// <summary>
    /// Force switch to English for testing
    /// </summary>
    public static void SwitchToEnglish()
    {
        Initialize("en");
    }

    /// <summary>
    /// Get current loaded language for debugging
    /// </summary>
    /// <returns>Debug info about current language</returns>
    public static string GetDebugInfo()
    {
        var systemLang = GetCurrentLanguageCode();
        var sampleText = GetString("AppTitle");
        return $"System: {CultureInfo.CurrentUICulture.Name}, Detected: {systemLang}, Sample: '{sampleText}'";
    }

    // Static properties for x:Bind support (no parameters allowed)
    public static string AppTitle => GetString("AppTitle");
    public static string Online => GetString("Online");
    public static string DiscoverPeers => GetString("DiscoverPeers");
    public static string Peers => GetString("Peers");
    public static string WelcomeTitle => GetString("WelcomeTitle");
    public static string WelcomeMessage => GetString("WelcomeMessage");
}