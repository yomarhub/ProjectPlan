using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjectPlan.ViewModels;

namespace ProjectPlan.Views;

public partial class ColumnDialog : Window
{
    private readonly ColumnDialogViewModel _vm = new();

    public ColumnDialog()
    {
        InitializeComponent();
        DataContext = _vm;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(new ColumnDialogResult(ColumnDialogAction.Cancel, string.Empty));
    }

    private void Create_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(new ColumnDialogResult(ColumnDialogAction.Create, (_vm.Name ?? string.Empty).Trim()));
    }
}
