using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjectPlan.ViewModels;

namespace ProjectPlan.Views;

public partial class ConfirmDeleteProjectDialog : Window
{
    private readonly ConfirmDeleteProjectDialogViewModel _vm;

    public ConfirmDeleteProjectDialog()
    {
        InitializeComponent();

        _vm = new ConfirmDeleteProjectDialogViewModel(projectName: string.Empty);
        _vm.RequestClose += OnRequestClose;
        Closed += (_, _) => _vm.RequestClose -= OnRequestClose;
        DataContext = _vm;
    }

    public ConfirmDeleteProjectDialog(string projectName) : this()
    {
        _vm.ProjectName = projectName;
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
