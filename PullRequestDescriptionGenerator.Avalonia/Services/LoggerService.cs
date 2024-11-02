using Serilog;
using System;
using System.IO;

namespace PullRequestDescriptor.Services;

public class LoggerService
{
    private readonly ILogger _logger;

    public LoggerService()
    {
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PullRequestDescriptor",
            "logs",
            "app-.log");

        _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        _logger.Information("Application started");
    }

    public void LogInformation(string message) => _logger.Information(message);
    public void LogWarning(string message) => _logger.Warning(message);
    public void LogError(Exception ex, string message) => _logger.Error(ex, message);
    public void LogDebug(string message) => _logger.Debug(message);
}
