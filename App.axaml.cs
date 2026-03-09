using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using ProjectPlan.ViewModels;
using ProjectPlan.Views;

namespace ProjectPlan;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            var mainVm = new MainWindowViewModel();
            desktop.MainWindow = new MainWindow { DataContext = mainVm };

            _ = InitializeAsync(mainVm);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static async System.Threading.Tasks.Task InitializeAsync(MainWindowViewModel mainVm)
    {
        try
        {
            await mainVm.InitializeAsync();
        }
        catch (System.Exception ex)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Erreur",
                "Impossible d'initialiser la base de données.\n\n" + ex.Message);

            await box.ShowAsync();
        }
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void OpenApp(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

        desktop.MainWindow?.Show();
        desktop.MainWindow?.WindowState = WindowState.Normal;
    }

    private void ExitApp(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    private void Toggle(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

        if (desktop.MainWindow == null) return;
        if (desktop.MainWindow.IsVisible) desktop.MainWindow?.Hide();
        else OpenApp(sender, e);
    }
}