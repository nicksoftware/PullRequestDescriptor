using Avalonia.Controls;
using Avalonia.Platform;
using System;

namespace PullRequestDescriptor.Services;

public class TrayIconService
{
    private TrayIcon? _trayIcon;
    private readonly SettingsService _settingsService;
    private readonly Window _mainWindow;
    private readonly Action _showSettings;

    public TrayIconService(SettingsService settingsService, Window mainWindow, Action showSettings)
    {
        _settingsService = settingsService;
        _mainWindow = mainWindow;
        _showSettings = showSettings;
        InitializeTrayIcon();
    }

    private void InitializeTrayIcon()
    {
        if (!_settingsService.CurrentSettings.ShowInMenuBar)
            return;

        _trayIcon = new TrayIcon
        {
            Icon = new WindowIcon("Assets/app-icon.png"),
            ToolTipText = "PR Description Generator",
            Menu = CreateTrayMenu()
        };

        _trayIcon.Clicked += (s, e) => ShowMainWindow();
    }

    private NativeMenu CreateTrayMenu()
    {
        var menu = new NativeMenu();

        var showItem = new NativeMenuItem("Show");
        showItem.Click += (s, e) => ShowMainWindow();
        menu.Add(showItem);

        var settingsItem = new NativeMenuItem("Settings...");
        settingsItem.Click += (s, e) => _showSettings();
        menu.Add(settingsItem);

        menu.Add(new NativeMenuItemSeparator());

        var quitItem = new NativeMenuItem("Quit");
        quitItem.Click += (s, e) => QuitApplication();
        menu.Add(quitItem);

        return menu;
    }

    private void ShowMainWindow()
    {
        _mainWindow.Show();
        _mainWindow.WindowState = WindowState.Normal;
        _mainWindow.Activate();
    }

    private void QuitApplication()
    {
        _trayIcon?.Dispose();
        Environment.Exit(0);
    }

    public void UpdateVisibility(bool show)
    {
        if (show)
        {
            if (_trayIcon == null)
            {
                InitializeTrayIcon();
            }
        }
        else
        {
            _trayIcon?.Dispose();
            _trayIcon = null;
        }
    }

    public void Dispose()
    {
        _trayIcon?.Dispose();
    }
}
