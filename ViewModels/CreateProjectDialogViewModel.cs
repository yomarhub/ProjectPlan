using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectPlan.ViewModels;

public sealed partial class CreateProjectDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _projectTitle;

    [ObservableProperty]
    private string? _projectDescription;

    [ObservableProperty]
    private string? _imagePath;

    [ObservableProperty]
    private IImage? _imagePreview;
}

public sealed record CreateProjectResult(
    string Title,
    string Description,
    string? ImagePath,
    IImage? Image);
