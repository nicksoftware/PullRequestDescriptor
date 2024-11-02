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

    [ObservableProperty]
    private string openAIModel;

    public IReadOnlyList<string> AvailableThemes { get; } = new[] { "System", "Light", "Dark" };

    public IReadOnlyList<string> AvailableModels { get; } = new[]
    {
        "gpt-4-turbo-preview",
        "gpt-4",
        "gpt-3.5-turbo",
        "gpt-3.5-turbo-16k"
    };

    public SettingsViewModel(SettingsService settingsService, ThemeService themeService)
    {
        _settingsService = settingsService;
        _themeService = themeService;

        var settings = settingsService.CurrentSettings;
        theme = settings.Theme;
        showInMenuBar = settings.ShowInMenuBar;
        apiKey = settings.ApiKey;
        promptTemplate = settings.PromptTemplate;
        openAIModel = settings.OpenAIModel;
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
            PromptTemplate = PromptTemplate,
            OpenAIModel = OpenAIModel
        };

        await _settingsService.SaveSettingsAsync(settings);
        _themeService.SetTheme(Theme);
    }
}
