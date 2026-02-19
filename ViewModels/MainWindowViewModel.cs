namespace ProjectPlan.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    private readonly DashboardViewModel _dashboard = new();
    private readonly ProjectViewModel _project = new();
    private readonly SettingsViewModel _settings = new();

    private int _newProjectIndex = 1;

    [ObservableProperty]
    private ViewModelBase _currentPage;

    public MainWindowViewModel()
    {
        _currentPage = _dashboard; // page par défaut
    }

    [RelayCommand]
    private void ShowDashboard() => CurrentPage = _dashboard;

    [RelayCommand]
    private void ShowProject() => CurrentPage = _project;

    [RelayCommand]
    private void OpenProjectFromCard(DashboardProjectCard card)
    {
        _project.ProjectName = card.Title;
        CurrentPage = _project;
    }

    [RelayCommand]
    private void CreateNewProject()
    {
        _project.ProjectName = $"Nouveau projet {_newProjectIndex++}";
        _project.ProjectDescription = string.Empty;
        _project.ProjectImage = null;
        CurrentPage = _project;
    }

    public void CreateNewProject(CreateProjectResult result)
    {
        var title = string.IsNullOrWhiteSpace(result.Title)
            ? $"Nouveau projet {_newProjectIndex++}"
            : result.Title.Trim();

        _project.ProjectName = title;
        _project.ProjectDescription = result.Description?.Trim() ?? string.Empty;
        _project.ProjectImage = result.Image;
        CurrentPage = _project;
    }

    [RelayCommand]
    private void ShowSettings() => CurrentPage = _settings;
}