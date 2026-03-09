using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProjectPlan.ViewModels;

public sealed partial class ConfirmDeleteColumnDialogViewModel : ViewModelBase
{
    public ConfirmDeleteColumnDialogViewModel(string columnName)
    {
        _columnName = columnName;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Message))]
    private string _columnName;

    public string Message =>
        $"Supprimer la colonne \"{ColumnName}\" ?\n\n" +
        "Cette action supprimera aussi son contenu (tickets).";

    public event System.Action<bool>? RequestClose;

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke(false);
    }

    [RelayCommand]
    private void Delete()
    {
        RequestClose?.Invoke(true);
    }
}
