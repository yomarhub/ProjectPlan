using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectPlan.ViewModels;

public sealed partial class CardDialogViewModel : ViewModelBase
{
    public CardDialogViewModel(bool isEditMode)
    {
        IsEditMode = isEditMode;
    }

    public bool IsEditMode { get; }

    public bool IsCreateMode => !IsEditMode;

    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private string? _description;
}

public enum CardDialogAction
{
    Save,
    Delete,
    Cancel,
}

public sealed record CardDialogResult(CardDialogAction Action, string Title, string Description);
