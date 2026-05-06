using MudBlazor;
using Metrobones.Models;

namespace Metrobones.Services;

public class ThemeManager(LocalStorage storage, ILogger<ThemeManager> logger)
{
    private MudTheme? _theme;
    private bool? _useDarkMode;
    private Dictionary<string, MudTheme>? _themeDict;
    private const string ThemeKey = "theme";
    private const string DarkModeKey = "useDarkMode";

    public event Action? OnThemeChanged;

    public Dictionary<string, MudTheme> ThemeDict => _themeDict ??= GetThemes();

    public MudTheme Theme
    {
        get
        {
            if (_theme == null)
            {
                logger.LogError("ThemeManager was not initialized! Using Mud theme.");
                return MudBlazorThemes.Mud;
            }
            return _theme;
        }
        set
        {
            _theme = value;
            OnThemeChanged?.Invoke();
            string themeName = ThemeDict.FirstOrDefault(x => x.Value == value).Key;
            _ = storage.SetAsync(ThemeKey, themeName);
        }
    }

    public bool UseDarkMode
    {
        get
        {
            if (_useDarkMode == null)
            {
                logger.LogError("ThemeManager was not initialized! Using light mode.");
                return false;
            }
            return (bool)_useDarkMode;
        }
        set
        {
            _useDarkMode = value;
            OnThemeChanged?.Invoke();
            _ = storage.SetAsync(DarkModeKey, value);
        }
    }

    public async Task InitializeAsync()
    {
        string themeName = await storage.GetAsync<string>(ThemeKey) ?? "";
        if(ThemeDict.TryGetValue(themeName, out MudTheme? theme) == false)
        {
            Theme = MudBlazorThemes.Mud;
            logger.LogWarning("Failed to load theme '{ThemeName}'. Using default Mud theme.", themeName);
        }
        else
        {
            _theme = theme;
        }

        UseDarkMode = await storage.GetAsync<bool>(DarkModeKey);
        logger.LogDebug("ThemeManager initialized! Theme: {ThemeName} DarkMode: {UseDarkMode} ", themeName, _useDarkMode);
    }

    private static Dictionary<string, MudTheme> GetThemes()
    {
        return typeof(MudBlazorThemes)
            .GetFields()
            .ToDictionary(
                f => f.Name,
                f => (MudTheme)f.GetValue(null)!
            );
    }
}