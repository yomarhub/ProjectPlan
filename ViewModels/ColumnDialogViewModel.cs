using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectPlan.ViewModels;

public sealed partial class ColumnDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _name;
}

public enum ColumnDialogAction
{
    Create,
    Cancel,
}

public sealed record ColumnDialogResult(ColumnDialogAction Action, string Name);
