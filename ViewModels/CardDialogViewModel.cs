using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProjectPlan.ViewModels;

public sealed partial class CardDialogViewModel : ViewModelBase
{
    public CardDialogViewModel(bool isEditMode)
    {
        IsEditMode = isEditMode;
    }

    public bool IsEditMode { get; }

    public bool IsCreateMode => !IsEditMode;

    public event System.Action<CardDialogResult>? RequestClose;

    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private string? _description;

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke(new CardDialogResult(CardDialogAction.Cancel, string.Empty, string.Empty));
    }

    [RelayCommand]
    private void Save()
    {
        RequestClose?.Invoke(new CardDialogResult(
            CardDialogAction.Save,
            (Title ?? string.Empty).Trim(),
            (Description ?? string.Empty).Trim()));
    }

    [RelayCommand]
    private void Delete()
    {
        RequestClose?.Invoke(new CardDialogResult(CardDialogAction.Delete, string.Empty, string.Empty));
    }
}

public enum CardDialogAction
{
    Save,
    Delete,
    Cancel,
}

public sealed record CardDialogResult(CardDialogAction Action, string Title, string Description);
