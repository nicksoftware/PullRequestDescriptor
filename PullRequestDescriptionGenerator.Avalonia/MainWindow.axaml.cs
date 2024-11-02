using Avalonia.Controls;
using PullRequestDescriptor.ViewModels;

namespace PullRequestDescriptor;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosed(System.EventArgs e)
    {
        base.OnClosed(e);
        if (DataContext is MainWindowViewModel vm)
        {
            vm.Cleanup();
        }
    }
}
