using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProjectPlan.ViewModels;

public sealed partial class ColumnDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _name;

    public event System.Action<ColumnDialogResult>? RequestClose;

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke(new ColumnDialogResult(ColumnDialogAction.Cancel, string.Empty));
    }

    [RelayCommand]
    private void Create()
    {
        RequestClose?.Invoke(new ColumnDialogResult(
            ColumnDialogAction.Create,
            (Name ?? string.Empty).Trim()));
    }
}

public enum ColumnDialogAction
{
    Create,
    Cancel,
}

public sealed record ColumnDialogResult(ColumnDialogAction Action, string Name);
