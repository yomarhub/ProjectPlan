using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectPlan.DataAccess;
using ProjectPlan.Infrastructure;
using ProjectPlan.Views;

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

    [ObservableProperty]
    private string? _projectThumbnailPath;

    public ProjectViewModel()
    {
        _projectImage = LoadDefaultImage();
    }

    private static Window? GetMainWindow()
    {
        return (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
    }

    private static IImage? LoadDefaultImage()
    {
        try
        {
            return new Bitmap(AssetLoader.Open(AssetUri.For("Assets/Default.png")));
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
        var project = await DataFunctions.GetProjectAsync(projectId, cancellationToken);
        if (project is null)
        {
            ProjectId = null;
            ProjectName = "(projet introuvable)";
            ProjectDescription = string.Empty;
            ProjectImage = LoadDefaultImage();
            ProjectThumbnailPath = null;
            Columns.Clear();
            return;
        }

        ProjectId = project.Id;
        ProjectName = project.Name;
        ProjectDescription = project.Description ?? string.Empty;
        ProjectThumbnailPath = project.Thumbnail;
        ProjectImage = LoadImageFromFileOrNull(project.Thumbnail) ?? LoadDefaultImage();

        await LoadBoardAsync(project.Id, cancellationToken);
    }

    [RelayCommand]
    private async Task EditProjectAsync()
    {
        if (ProjectId is null)
            return;

        var window = GetMainWindow();
        if (window is null)
            return;

        var dialog = new EditProjectDialog(
            initialTitle: ProjectName,
            initialDescription: ProjectDescription,
            initialImagePath: ProjectThumbnailPath)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        var result = await dialog.ShowDialog<EditProjectResult?>(window);
        if (result is null)
            return;

        if (string.IsNullOrWhiteSpace(result.Title))
            return;

        var updated = await DataFunctions.UpdateProjectAsync(
            ProjectId.Value,
            name: result.Title,
            description: result.Description,
            thumbnailPath: result.ImagePath);

        if (updated is null)
            return;

        ProjectName = updated.Name;
        ProjectDescription = updated.Description ?? string.Empty;
        ProjectThumbnailPath = updated.Thumbnail;
        ProjectImage = LoadImageFromFileOrNull(updated.Thumbnail) ?? LoadDefaultImage();
    }

    private async Task LoadBoardAsync(int projectId, CancellationToken cancellationToken)
    {
        var columns = await DataFunctions.GetBoardColumnsAsync(projectId, cancellationToken);

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

    [RelayCommand]
    private async Task DeleteProjectAsync()
    {
        if (ProjectId is null)
            return;

        var window = GetMainWindow();
        if (window is null)
            return;

        var confirm = new ConfirmDeleteProjectDialog(ProjectName)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        var shouldDelete = await confirm.ShowDialog<bool>(window);
        if (!shouldDelete)
            return;

        var deleted = await DataFunctions.DeleteProjectAsync(ProjectId.Value);
        if (!deleted)
            return;

        if (window.DataContext is MainWindowViewModel mainVm)
            await mainVm.ShowDashboardAndRefreshAsync();
    }

    [RelayCommand]
    private async Task AddColumnAsync()
    {
        if (ProjectId is null)
            return;

        var window = GetMainWindow();
        if (window is null)
            return;

        var dialog = new ColumnDialog
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        var result = await dialog.ShowDialog<ColumnDialogResult?>(window);
        if (result is null || result.Action != ColumnDialogAction.Create)
            return;

        if (string.IsNullOrWhiteSpace(result.Name))
            return;

        var column = await DataFunctions.CreateColumnAsync(ProjectId.Value, result.Name, color: null);
        Columns.Add(new BoardColumnViewModel(column.Id, column.Name));
    }

    [RelayCommand]
    private async Task DeleteColumnAsync(BoardColumnViewModel? columnVm)
    {
        if (columnVm is null)
            return;

        var deleted = await DataFunctions.DeleteColumnAsync(columnVm.Id);
        if (!deleted)
            return;

        Columns.Remove(columnVm);
    }

    [RelayCommand]
    private async Task AddCardAsync(BoardColumnViewModel? columnVm)
    {
        if (columnVm is null)
            return;

        var window = GetMainWindow();
        if (window is null)
            return;

        var dialog = new CardDialog(isEditMode: false)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        var result = await dialog.ShowDialog<CardDialogResult?>(window);
        if (result is null || result.Action != CardDialogAction.Save)
            return;

        if (string.IsNullOrWhiteSpace(result.Title))
            return;

        var card = await DataFunctions.CreateCardAsync(
            columnId: columnVm.Id,
            title: result.Title,
            description: result.Description,
            color: null);

        columnVm.Cards.Add(new BoardCardViewModel(card.Id, columnVm.Id, card.Title, card.Description));
    }

    [RelayCommand]
    private async Task MoveCardLeftAsync(BoardCardViewModel? cardVm)
    {
        if (cardVm is null)
            return;

        var currentIndex = IndexOfColumn(Columns, cardVm.ColumnId);
        if (currentIndex <= 0)
            return;

        var sourceColumnVm = Columns[currentIndex];
        var targetColumnVm = Columns[currentIndex - 1];

        var moved = await DataFunctions.MoveCardToColumnAsync(cardVm.Id, targetColumnVm.Id);
        if (!moved)
            return;

        sourceColumnVm.Cards.Remove(cardVm);
        targetColumnVm.Cards.Add(cardVm);
        cardVm.ColumnId = targetColumnVm.Id;
    }

    [RelayCommand]
    private async Task MoveCardRightAsync(BoardCardViewModel? cardVm)
    {
        if (cardVm is null)
            return;

        var currentIndex = IndexOfColumn(Columns, cardVm.ColumnId);
        if (currentIndex < 0 || currentIndex >= Columns.Count - 1)
            return;

        var sourceColumnVm = Columns[currentIndex];
        var targetColumnVm = Columns[currentIndex + 1];

        var moved = await DataFunctions.MoveCardToColumnAsync(cardVm.Id, targetColumnVm.Id);
        if (!moved)
            return;

        sourceColumnVm.Cards.Remove(cardVm);
        targetColumnVm.Cards.Add(cardVm);
        cardVm.ColumnId = targetColumnVm.Id;
    }

    private static int IndexOfColumn(ObservableCollection<BoardColumnViewModel> columns, int columnId)
    {
        for (var i = 0; i < columns.Count; i++)
        {
            if (columns[i].Id == columnId)
                return i;
        }

        return -1;
    }

    [RelayCommand]
    private async Task CardAsync(BoardCardViewModel? cardVm)
    {
        if (cardVm is null)
            return;

        var columnVm = Columns.FirstOrDefault(c => c.Id == cardVm.ColumnId);
        if (columnVm is null)
            return;

        var window = GetMainWindow();
        if (window is null)
            return;

        var dialog = new CardDialog(isEditMode: true, title: cardVm.Title, description: cardVm.Description)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        var result = await dialog.ShowDialog<CardDialogResult?>(window);
        if (result is null)
            return;

        if (result.Action == CardDialogAction.Cancel)
            return;

        if (result.Action == CardDialogAction.Delete)
        {
            var deleted = await DataFunctions.DeleteCardAsync(cardVm.Id);
            if (deleted)
                columnVm.Cards.Remove(cardVm);
            return;
        }

        if (result.Action == CardDialogAction.Save)
        {
            if (string.IsNullOrWhiteSpace(result.Title))
                return;

            var updated = await DataFunctions.UpdateCardAsync(cardVm.Id, result.Title, result.Description);
            if (updated is null)
                return;

            cardVm.Title = updated.Title;
            cardVm.Description = updated.Description ?? string.Empty;
        }
    }
}
