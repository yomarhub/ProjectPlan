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

        DataContext = _vm;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(new CardDialogResult(CardDialogAction.Cancel, string.Empty, string.Empty));
    }

    private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(new CardDialogResult(
            CardDialogAction.Save,
            (_vm.Title ?? string.Empty).Trim(),
            (_vm.Description ?? string.Empty).Trim()));
    }

    private void Delete_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(new CardDialogResult(CardDialogAction.Delete, string.Empty, string.Empty));
    }
}
