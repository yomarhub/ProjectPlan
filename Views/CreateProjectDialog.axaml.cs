using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjectPlan.ViewModels;

namespace ProjectPlan.Views;

public partial class CreateProjectDialog : Window
{
    private readonly CreateProjectDialogViewModel _vm;

    public CreateProjectDialog()
    {
        InitializeComponent();

        _vm = new CreateProjectDialogViewModel(() => StorageProvider);
        _vm.RequestClose += OnRequestClose;
        Closed += (_, _) => _vm.RequestClose -= OnRequestClose;

        DataContext = _vm;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnRequestClose(CreateProjectResult? result)
    {
        Close(result);
    }
}
