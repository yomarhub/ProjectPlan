using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjectPlan.ViewModels;

namespace ProjectPlan.Views;

public partial class ConfirmDeleteColumnDialog : Window
{
    private readonly ConfirmDeleteColumnDialogViewModel _vm;

    public ConfirmDeleteColumnDialog()
    {
        InitializeComponent();

        _vm = new ConfirmDeleteColumnDialogViewModel(columnName: string.Empty);
        _vm.RequestClose += OnRequestClose;
        Closed += (_, _) => _vm.RequestClose -= OnRequestClose;
        DataContext = _vm;
    }

    public ConfirmDeleteColumnDialog(string columnName) : this()
    {
        _vm.ColumnName = columnName;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnRequestClose(bool result)
    {
        Close(result);
    }
}
