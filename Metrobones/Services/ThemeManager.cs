using MudBlazor;
using Metrobones.Models;

namespace Metrobones.Services;

public class ThemeManager
{
    public MudTheme Theme = MudBlazorThemes.MudBlazorDefault;
    public bool IsDarkMode = true;

    public void SetDarkMode(bool isDarkMode)
    {
        IsDarkMode = isDarkMode;
    }

    public void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
    }

    public void SetTheme(MudTheme theme)
    {
        Theme = theme;
    }
}