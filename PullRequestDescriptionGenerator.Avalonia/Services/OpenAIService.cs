using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PullRequestDescriptor.Services;

public class OpenAIService
{
    private readonly Kernel _kernel;
    private readonly SettingsService _settingsService;
    private const string ModelName = "gpt-4o-mini";
    private const int MaxChunkSize = 8000; // Safe limit for context window

    public OpenAIService(string apiKey, SettingsService settingsService)
    {
        _settingsService = settingsService;
        var builder = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(ModelName, apiKey);
        _kernel = builder.Build();
    }

    public async Task<string> GeneratePullRequestDescription(string changes)
    {
        try
        {
            // First, summarize large changes if needed
            var processedChanges = await SummarizeIfTooLarge(changes);

            // Get the template from settings
            var promptTemplate = _settingsService.CurrentSettings.PromptTemplate;
            
            // If template is empty, use the default template
            if (string.IsNullOrWhiteSpace(promptTemplate))
            {
                promptTemplate = SettingsService.DefaultPromptTemplate;
            }

            var function = _kernel.CreateFunctionFromPrompt(
                promptTemplate,
                new OpenAIPromptExecutionSettings
                {
                    MaxTokens = 2000,
                    Temperature = 0.7f,
                    TopP = 0.95f,
                });

            var result = await _kernel.InvokeAsync(function, new KernelArguments { ["input"] = processedChanges });
            return result.GetValue<string>() ?? "Failed to generate description.";
        }
        catch (Exception ex)
        {
            throw new Exception($"Error generating PR description: {ex.Message}", ex);
        }
    }

    private async Task<string> SummarizeIfTooLarge(string changes)
    {
        var lines = changes.Split('\n');
        if (lines.Length <= MaxChunkSize)
        {
            return changes;
        }

        // Split changes into manageable chunks
        var chunks = ChunkChanges(changes);
        var summaries = new StringBuilder();

        foreach (var chunk in chunks)
        {
            var summaryPrompt = @"Summarize the following code changes concisely, focusing on the key modifications:

                {{$input}}

                Provide a brief technical summary of the changes.";

            var summaryFunction = _kernel.CreateFunctionFromPrompt(
                summaryPrompt,
                new OpenAIPromptExecutionSettings
                {
                    MaxTokens = 500,
                    Temperature = 0.3f,
                    TopP = 0.95f
                });

            var summary = await _kernel.InvokeAsync(summaryFunction, new KernelArguments { ["input"] = chunk });
            summaries.AppendLine(summary.GetValue<string>());
        }

        return summaries.ToString();
    }

    private static string[] ChunkChanges(string changes)
    {
        var lines = changes.Split('\n');
        var chunks = new System.Collections.Generic.List<string>();
        var currentChunk = new StringBuilder();
        var currentFileChanges = new StringBuilder();
        var linesInCurrentChunk = 0;

        foreach (var line in lines)
        {
            if (line.StartsWith("File: "))
            {
                // If we have accumulated changes and reached the chunk size limit
                if (linesInCurrentChunk >= MaxChunkSize)
                {
                    if (currentFileChanges.Length > 0)
                    {
                        currentChunk.Append(currentFileChanges);
                    }
                    chunks.Add(currentChunk.ToString());
                    currentChunk.Clear();
                    linesInCurrentChunk = 0;
                }

                // Add previous file changes if any
                if (currentFileChanges.Length > 0)
                {
                    currentChunk.Append(currentFileChanges);
                    currentFileChanges.Clear();
                }

                // Start new file changes
                currentFileChanges.AppendLine(line);
                linesInCurrentChunk++;
            }
            else
            {
                currentFileChanges.AppendLine(line);
                linesInCurrentChunk++;
            }
        }

        // Add remaining changes
        if (currentFileChanges.Length > 0)
        {
            currentChunk.Append(currentFileChanges);
        }
        if (currentChunk.Length > 0)
        {
            chunks.Add(currentChunk.ToString());
        }

        return chunks.ToArray();
    }
}
