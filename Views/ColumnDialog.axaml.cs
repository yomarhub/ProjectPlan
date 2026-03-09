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

        _vm.RequestClose += OnRequestClose;
        Closed += (_, _) => _vm.RequestClose -= OnRequestClose;

        DataContext = _vm;
    }

    private void OnRequestClose(ColumnDialogResult result)
    {
        Close(result);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
