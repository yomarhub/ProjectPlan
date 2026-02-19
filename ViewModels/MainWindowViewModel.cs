namespace ProjectPlan.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    private readonly DashboardViewModel _dashboard = new();
    private readonly ProjectViewModel _project = new();
    private readonly SettingsViewModel _settings = new();

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
    private void OpenProjectFromCard(DashboardCard card)
    {
        _project.ProjectId = card.Id;
        CurrentPage = _project;
    }

    [RelayCommand]
    private void ShowSettings() => CurrentPage = _settings;
}