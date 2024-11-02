using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibGit2Sharp;
using PullRequestDescriptor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PullRequestDescriptor.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;
    private readonly OpenAIService _openAIService;
    private Repository? _repository;

    [ObservableProperty]
    private string repositoryPath = string.Empty;

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private string outputText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> branches = new();

    [ObservableProperty]
    private string? selectedBranch;

    [ObservableProperty]
    private bool isGenerating;

    public MainWindowViewModel(SettingsService settingsService, OpenAIService openAIService)
    {
        _settingsService = settingsService;
        _openAIService = openAIService;

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(RepositoryPath))
            {
                LoadRepositoryCommand.Execute(null);
            }
        };
    }

    [RelayCommand]
    private async Task BrowseRepository(Window window)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select Git Repository"
        };

        var result = await dialog.ShowAsync(window);
        if (!string.IsNullOrWhiteSpace(result))
        {
            RepositoryPath = result;
        }
    }

    [RelayCommand]
    private void LoadRepository()
    {
        if (string.IsNullOrWhiteSpace(RepositoryPath))
        {
            StatusMessage = "Please select a repository";
            return;
        }

        try
        {
            _repository?.Dispose();
            _repository = new Repository(RepositoryPath);
            LoadBranches();
            StatusMessage = "Repository loaded successfully";
        }
        catch (RepositoryNotFoundException)
        {
            StatusMessage = "Invalid Git repository path";
            ClearBranches();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading repository: {ex.Message}";
            ClearBranches();
        }
    }

    private void LoadBranches()
    {
        if (_repository == null) return;

        var branchList = _repository.Branches
            .Select(b => b.FriendlyName)
            .OrderBy(name => name)
            .ToList();

        Branches.Clear();
        foreach (var branch in branchList)
        {
            Branches.Add(branch);
        }

        SelectedBranch = _repository.Head.FriendlyName;
    }

    private void ClearBranches()
    {
        Branches.Clear();
        SelectedBranch = null;
    }

    [RelayCommand]
    private async Task GenerateDescription()
    {
        if (_repository == null)
        {
            StatusMessage = "Please select a valid repository";
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedBranch))
        {
            StatusMessage = "Please select a branch";
            return;
        }

        try
        {
            IsGenerating = true;
            StatusMessage = "Generating description...";

            var changes = GetChangesForBranch(SelectedBranch);
            var changesText = string.Join("\n", changes);

            // First show the raw changes
            OutputText = changesText;

            // Then generate and append the AI description
            var description = await _openAIService.GeneratePullRequestDescription(changesText);

            OutputText = $"# AI Generated Pull Request Description\n\n{description}\n\n" +
                        $"# Raw Changes\n\n{changesText}";

            StatusMessage = "Description generated successfully";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            OutputText = $"Error generating description: {ex.Message}";
        }
        finally
        {
            IsGenerating = false;
        }
    }

    private List<string> GetChangesForBranch(string branchName)
    {
        if (_repository == null)
            return [];

        var branch = _repository.Branches[branchName];
        if (branch == null)
            return [];

        List<string> changes = [];

        var mainBranch = _repository.Branches["main"] ?? _repository.Branches["master"];
        if (mainBranch == null)
        {
            changes.Add("Could not find main or master branch");
            return changes;
        }

        var commonAncestor = _repository.ObjectDatabase.FindMergeBase(branch.Tip, mainBranch.Tip);
        if (commonAncestor == null)
        {
            changes.Add("Could not find common ancestor with main/master branch");
            return changes;
        }

        var diff = _repository.Diff.Compare<Patch>(commonAncestor.Tree, branch.Tip.Tree);

        foreach (var change in diff)
        {
            changes.Add($"File: {change.Path}");
            changes.Add($"Status: {change.Status}");
            changes.Add(change.Patch);
            changes.Add("-------------------");
        }

        return changes;
    }

    public void Cleanup()
    {
        _repository?.Dispose();
    }
}
