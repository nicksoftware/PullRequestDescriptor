using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PullRequestDescriptor.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PullRequestDescriptor.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;
    private readonly ThemeService _themeService;

    [ObservableProperty]
    private string theme;

    [ObservableProperty]
    private bool showInMenuBar;

    [ObservableProperty]
    private string apiKey;

    [ObservableProperty]
    private string promptTemplate;

    public IReadOnlyList<string> AvailableThemes { get; } = new[] { "System", "Light", "Dark" };

    public SettingsViewModel(SettingsService settingsService, ThemeService themeService)
    {
        _settingsService = settingsService;
        _themeService = themeService;

        var settings = settingsService.CurrentSettings;
        theme = settings.Theme;
        showInMenuBar = settings.ShowInMenuBar;
        apiKey = settings.ApiKey;
        promptTemplate = settings.PromptTemplate;
    }

    [RelayCommand]
    private void ResetPromptTemplate()
    {
        PromptTemplate = SettingsService.DefaultPromptTemplate;
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        var settings = new Settings
        {
            Theme = Theme,
            ShowInMenuBar = ShowInMenuBar,
            ApiKey = ApiKey,
            PromptTemplate = PromptTemplate
        };

        await _settingsService.SaveSettingsAsync(settings);
        _themeService.SetTheme(Theme);
    }
}
