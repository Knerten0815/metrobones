using MudBlazor;
using Metrobones.Models;

namespace Metrobones.Services;

public class ThemeManager
{
    private MudTheme _theme = null!;
    private bool _isDarkMode = false;

    public event Action? OnThemeChanged;

    public MudTheme Theme
    {
        get
        {
            if (_theme == null)
            {
                _theme = MudBlazorThemes.Mud;
            }
            return _theme;
        }
        set
        {
            _theme = value;
            OnThemeChanged?.Invoke();
        }
    }

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            _isDarkMode = value;
            OnThemeChanged?.Invoke();
        }
    }

    public static Dictionary<string, MudTheme> GetThemes()
    {
        Dictionary<string, MudTheme> themes = typeof(MudBlazorThemes)
            .GetFields()
            .ToDictionary(
                f => f.Name,
                f => (MudTheme)f.GetValue(null)!
            );
        return themes;
    }
}