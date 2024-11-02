using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PullRequestDescriptor.Services;
using PullRequestDescriptor.ViewModels;
using PullRequestDescriptor.Views;
using System;
using System.Threading.Tasks;

namespace PullRequestDescriptor;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;
    private LoggerService? _logger;

    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // Set up global exception handling
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var exception = (Exception)args.ExceptionObject;
            _logger?.LogError(exception, "Unhandled application exception");
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            _logger?.LogError(args.Exception, "Unobserved task exception");
            args.SetObserved();
        };

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
        Services = _serviceProvider;

        _logger = _serviceProvider.GetRequiredService<LoggerService>();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<LoggerService>();
        services.AddSingleton<ErrorHandler>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<ThemeService>();
        services.AddTransient(provider =>
        {
            var settingsService = provider.GetRequiredService<SettingsService>();
            return new OpenAIService(settingsService.CurrentSettings.ApiKey, settingsService);
        });
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<MainWindowViewModel>();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = _serviceProvider?.GetRequiredService<MainWindowViewModel>()
            };

            // Set up keyboard shortcuts
            desktop.MainWindow.KeyDown += (s, e) =>
            {
                if (e.Key == Avalonia.Input.Key.OemComma && e.KeyModifiers == Avalonia.Input.KeyModifiers.Meta)
                {
                    ShowSettings();
                }
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ShowSettings()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var settingsWindow = new SettingsWindow
            {
                DataContext = _serviceProvider?.GetService<SettingsViewModel>()
            };

            settingsWindow.ShowDialog(desktop.MainWindow);
        }
    }
}
