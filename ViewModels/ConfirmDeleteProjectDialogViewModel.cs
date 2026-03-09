using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProjectPlan.ViewModels;

public sealed partial class ConfirmDeleteProjectDialogViewModel : ViewModelBase
{
    public ConfirmDeleteProjectDialogViewModel(string projectName)
    {
        _projectName = projectName;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Message))]
    private string _projectName;

    public string Message =>
        $"Supprimer le projet \"{ProjectName}\" ?\n\n" +
        "Cette action supprimera aussi son contenu (colonnes et tickets).";

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
