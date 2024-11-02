using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PullRequestDescriptor.Services;

public class SettingsService
{
    private const string SettingsFileName = "settings.json";
    private readonly string _settingsPath;
    private Settings _currentSettings;

    public event EventHandler<Settings>? SettingsChanged;

    public SettingsService()
    {
        _settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PullRequestDescriptor",
            SettingsFileName);

        _currentSettings = LoadSettings();
    }

    public Settings CurrentSettings => _currentSettings;

    private Settings LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                return JsonSerializer.Deserialize<Settings>(json) ?? CreateDefaultSettings();
            }
        }
        catch (Exception)
        {
            // Log error if needed
        }

        return CreateDefaultSettings();
    }

    public async Task SaveSettingsAsync(Settings settings)
    {
        try
        {
            var directory = Path.GetDirectoryName(_settingsPath);
            if (!Directory.Exists(directory) && directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_settingsPath, json);
            _currentSettings = settings;

            SettingsChanged?.Invoke(this, settings);
        }
        catch (Exception)
        {
            // Log error if needed
            throw;
        }
    }

    private static Settings CreateDefaultSettings() => new()
    {
        Theme = "System",
        ShowInMenuBar = true,
        ApiKey = string.Empty,
        PromptTemplate = DefaultPromptTemplate
    };

    public static string DefaultPromptTemplate => @"You are a helpful assistant that generates clear and concise pull request descriptions based on code changes.
        Analyze the following code changes and generate a detailed pull request description following this specific template:

        ### 🔗 Related Issues
        [Analyze the changes and suggest potential related issues or leave as TBD if unclear]

        ### 🛠 Technical Changes
        [Provide a technical summary of the changes, focusing on implementation details, architecture changes, and technical decisions]

        ### 📝 Release Notes
        [Categorize the changes and describe their impact. Use the following format:]

        Select one or more categories that apply:
        - [ ] Added new feature
        - [ ] Fixed a bug
        - [ ] Improved performance
        - [ ] Other (Detail)

        Detailed descriptions:
        [List each significant change with its customer impact, following this format:]
        - **[Change Type]**: [Description of the change], [Impact on users/system]

        ### 📸 Testing Evidence
        [Based on the changes, suggest what kind of testing evidence would be appropriate]

        ### 🏎 Quality Checklist
        - [ ] Linter passed
        - [ ] No sensitive information logged
        - [ ] Confirmed no PII information is logged
        - [ ] Code is clean and clear, no unnecessary logs";
}

public class Settings
{
    public string Theme { get; set; } = "System";
    public bool ShowInMenuBar { get; set; } = true;
    public string ApiKey { get; set; } = string.Empty;
    public string PromptTemplate { get; set; } = string.Empty;
}
