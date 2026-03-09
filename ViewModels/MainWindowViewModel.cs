namespace ProjectPlan.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectPlan.Services;
using System;
using System.Threading.Tasks;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    private readonly DashboardViewModel _dashboard;
    private readonly ProjectViewModel _project;
    private readonly SettingsViewModel _settings = new();

    [ObservableProperty]
    private ViewModelBase _currentPage;

    public MainWindowViewModel()
    {
        _dashboard = new DashboardViewModel();
        _project = new ProjectViewModel();
        _currentPage = _dashboard; // page par défaut
    }

    public async Task InitializeAsync()
    {
        await _dashboard.RefreshAsync();
    }

    public async Task ShowDashboardAndRefreshAsync()
    {
        CurrentPage = _dashboard;
        await _dashboard.RefreshAsync();
    }

    [RelayCommand]
    private void ShowDashboard() => CurrentPage = _dashboard;

    [RelayCommand]
    private void ShowProject() => CurrentPage = _project;

    [RelayCommand]
    private async Task OpenProjectFromCard(DashboardProjectCard card)
    {
        if (card.ProjectId <= 0)
        {
            // sample tiles (fake data)
            _project.ProjectId = null;
            _project.ProjectName = card.Title;
            _project.ProjectDescription = card.Description;
            _project.ProjectImage = card.Image;
            CurrentPage = _project;
            return;
        }

        await _project.LoadProjectAsync(card.ProjectId);
        CurrentPage = _project;
    }

    [RelayCommand]
    private async Task CreateNewProject()
    {
        var project = await ProjectService.CreateProjectAsync(
            name: $"Nouveau projet {DateTime.Now:yyyy-MM-dd HH:mm}",
            description: null,
            thumbnailPath: null,
            background: null);

        await _dashboard.RefreshAsync();
        await _project.LoadProjectAsync(project.Id);
        CurrentPage = _project;
    }

    public async Task CreateNewProjectAsync(CreateProjectResult result)
    {
        var title = string.IsNullOrWhiteSpace(result.Title)
            ? $"Nouveau projet {DateTime.Now:yyyy-MM-dd HH:mm}"
            : result.Title.Trim();

        var project = await ProjectService.CreateProjectAsync(
            name: title,
            description: result.Description,
            thumbnailPath: result.ImagePath,
            background: null);

        await _dashboard.RefreshAsync();
        await _project.LoadProjectAsync(project.Id);
        CurrentPage = _project;
    }

    [RelayCommand]
    private void ShowSettings() => CurrentPage = _settings;
}