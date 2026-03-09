using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjectPlan.ViewModels;

namespace ProjectPlan.Views;

public partial class EditProjectDialog : Window
{
    private readonly EditProjectDialogViewModel _vm;

    public EditProjectDialog() : this(string.Empty, string.Empty, null)
    {
    }

    public EditProjectDialog(string initialTitle, string initialDescription, string? initialImagePath)
    {
        InitializeComponent();

        _vm = new EditProjectDialogViewModel(
            initialTitle,
            initialDescription,
            initialImagePath,
            () => StorageProvider);

        _vm.RequestClose += OnRequestClose;
        Closed += (_, _) => _vm.RequestClose -= OnRequestClose;

        DataContext = _vm;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnRequestClose(EditProjectResult? result)
    {
        Close(result);
    }
}
