using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Media;

namespace ProjectPlan.ViewModels;

public partial class ProjectViewModel : ViewModelBase
{
    public string Title { get; } = "Project";
    public string Hint { get; } = "tableau kanban à venir...";

    [ObservableProperty]
    private string _projectName = "(aucun projet sélectionné)";

    [ObservableProperty]
    private string _projectDescription = string.Empty;

    [ObservableProperty]
    private IImage? _projectImage;
}
