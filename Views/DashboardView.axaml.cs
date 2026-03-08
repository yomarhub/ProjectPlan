using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjectPlan.ViewModels;

namespace ProjectPlan.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    private async void AddProject_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as Window;
        if (window?.DataContext is not MainWindowViewModel mainVm)
            return;

        var dialog = new CreateProjectDialog
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        var result = await dialog.ShowDialog<CreateProjectResult?>(window);
        if (result is null)
            return;

        await mainVm.CreateNewProjectAsync(result);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}