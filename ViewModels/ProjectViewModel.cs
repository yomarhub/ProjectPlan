using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectPlan.ViewModels;

public partial class ProjectViewModel : ViewModelBase
{
    public string Title { get; } = "Project";
    public string Hint { get; } = "Ici tu mettras le contenu de tes projets.";

    [ObservableProperty]
    private string _projectName = "(aucun projet sélectionné)";
}
