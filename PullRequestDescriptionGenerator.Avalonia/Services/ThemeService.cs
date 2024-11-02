using Avalonia;
using Avalonia.Styling;
using System;

namespace PullRequestDescriptor.Services;

public class ThemeService
{
    public void SetTheme(string theme)
    {
        var app = Application.Current;
        if (app == null) return;

        app.RequestedThemeVariant = theme.ToLower() switch
        {
            "light" => ThemeVariant.Light,
            "dark" => ThemeVariant.Dark,
            _ => null // System default
        };
    }
}
