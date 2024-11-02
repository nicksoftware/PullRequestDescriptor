using System;
using System.Threading.Tasks;

namespace PullRequestDescriptor.Services;

public class ErrorHandler
{
    private readonly LoggerService _logger;

    public ErrorHandler(LoggerService logger)
    {
        _logger = logger;
    }

    public async Task<T> HandleAsync<T>(Func<Task<T>> action, string operation)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during {operation}");
            throw new ApplicationException($"Error during {operation}: {ex.Message}", ex);
        }
    }

    public void Handle(Action action, string operation)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during {operation}");
            throw new ApplicationException($"Error during {operation}: {ex.Message}", ex);
        }
    }
}
