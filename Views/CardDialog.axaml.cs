using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjectPlan.ViewModels;

namespace ProjectPlan.Views;

public partial class CardDialog : Window
{
    private readonly CardDialogViewModel _vm;

    public CardDialog() : this(isEditMode: false)
    {
    }

    public CardDialog(bool isEditMode, string? title = null, string? description = null)
    {
        InitializeComponent();

        _vm = new CardDialogViewModel(isEditMode)
        {
            Title = title ?? string.Empty,
            Description = description ?? string.Empty,
        };

        _vm.RequestClose += OnRequestClose;
        Closed += (_, _) => _vm.RequestClose -= OnRequestClose;

        DataContext = _vm;
    }

    private void OnRequestClose(CardDialogResult result)
    {
        Close(result);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
