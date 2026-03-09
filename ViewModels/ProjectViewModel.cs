using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ProjectPlan.Infrastructure;
using ProjectPlan.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace ProjectPlan.ViewModels;

public partial class ProjectViewModel : ViewModelBase
{
    public string Title { get; } = "Project";
    public string Hint { get; } = "tableau kanban à venir...";

    [ObservableProperty]
    private int? _projectId;

    public bool HasProject => ProjectId is not null;

    partial void OnProjectIdChanged(int? value)
    {
        OnPropertyChanged(nameof(HasProject));
    }

    public ObservableCollection<BoardColumnViewModel> Columns { get; } = new();

    [ObservableProperty]
    private string _projectName = "(aucun projet sélectionné)";

    [ObservableProperty]
    private string _projectDescription = string.Empty;

    [ObservableProperty]
    private IImage? _projectImage;

    public ProjectViewModel()
    {
        _projectImage = LoadDefaultImage();
    }

    private static IImage? LoadDefaultImage()
    {
        try
        {
            return new Bitmap(AssetLoader.Open(AssetUri.For("Assets/avalonia-logo.ico")));
        }
        catch
        {
            return null;
        }
    }

    private static IImage? LoadImageFromFileOrNull(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return null;

        try
        {
            if (!File.Exists(filePath))
                return null;

            return new Bitmap(filePath);
        }
        catch
        {
            return null;
        }
    }

    public async Task LoadProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var project = await ProjectService.GetProjectAsync(projectId, cancellationToken);
        if (project is null)
        {
            ProjectId = null;
            ProjectName = "(projet introuvable)";
            ProjectDescription = string.Empty;
            ProjectImage = LoadDefaultImage();
            Columns.Clear();
            return;
        }

        ProjectId = project.Id;
        ProjectName = project.Name;
        ProjectDescription = project.Description ?? string.Empty;
        ProjectImage = LoadImageFromFileOrNull(project.Thumbnail) ?? LoadDefaultImage();

        await LoadBoardAsync(project.Id, cancellationToken);
    }

    private async Task LoadBoardAsync(int projectId, CancellationToken cancellationToken)
    {
        var columns = await ProjectService.GetBoardColumnsAsync(projectId, cancellationToken);

        Columns.Clear();

        foreach (var column in columns)
        {
            var columnVm = new BoardColumnViewModel(column.Id, column.Name);

            foreach (var card in column.Cards)
            {
                columnVm.Cards.Add(new BoardCardViewModel(card.Id, column.Id, card.Title, card.Description));
            }

            Columns.Add(columnVm);
        }
    }


}
