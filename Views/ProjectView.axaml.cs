using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjectPlan.Services;
using ProjectPlan.ViewModels;
using System.Linq;

namespace ProjectPlan.Views;

public partial class ProjectView : UserControl
{
    public ProjectView()
    {
        InitializeComponent();
    }

    private async void DeleteColumn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ProjectViewModel projectVm)
            return;

        if (sender is not Control control || control.DataContext is not BoardColumnViewModel columnVm)
            return;

        var deleted = await ProjectService.DeleteColumnAsync(columnVm.Id);
        if (!deleted)
            return;

        projectVm.Columns.Remove(columnVm);
    }

    private async void DeleteProject_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ProjectViewModel projectVm)
            return;

        if (projectVm.ProjectId is null)
            return;

        var window = TopLevel.GetTopLevel(this) as Window;
        if (window is null)
            return;

        var confirm = new ConfirmDeleteProjectDialog(projectVm.ProjectName)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        var shouldDelete = await confirm.ShowDialog<bool>(window);
        if (!shouldDelete)
            return;

        var deleted = await ProjectService.DeleteProjectAsync(projectVm.ProjectId.Value);
        if (!deleted)
            return;

        // Navigate back to dashboard and refresh list.
        if (window.DataContext is MainWindowViewModel mainVm)
            await mainVm.ShowDashboardAndRefreshAsync();
    }

    private async void AddColumn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ProjectViewModel projectVm)
            return;

        if (projectVm.ProjectId is null)
            return;

        var window = TopLevel.GetTopLevel(this) as Window;
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

        var column = await ProjectService.CreateColumnAsync(projectVm.ProjectId.Value, result.Name, color: null);
        projectVm.Columns.Add(new BoardColumnViewModel(column.Id, column.Name));
    }

    private async void AddCard_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ProjectViewModel)
            return;

        if (sender is not Control control || control.DataContext is not BoardColumnViewModel columnVm)
            return;

        var window = TopLevel.GetTopLevel(this) as Window;
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

        var card = await ProjectService.CreateCardAsync(
            columnId: columnVm.Id,
            title: result.Title,
            description: result.Description,
            color: null);

        columnVm.Cards.Add(new BoardCardViewModel(card.Id, columnVm.Id, card.Title, card.Description));
    }

    private async void Card_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ProjectViewModel projectVm)
            return;

        if (sender is not Control control || control.DataContext is not BoardCardViewModel cardVm)
            return;

        var columnVm = projectVm.Columns.FirstOrDefault(c => c.Id == cardVm.ColumnId);
        if (columnVm is null)
            return;

        var window = TopLevel.GetTopLevel(this) as Window;
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
            var deleted = await ProjectService.DeleteCardAsync(cardVm.Id);
            if (deleted)
                columnVm.Cards.Remove(cardVm);
            return;
        }

        if (result.Action == CardDialogAction.Save)
        {
            if (string.IsNullOrWhiteSpace(result.Title))
                return;

            var updated = await ProjectService.UpdateCardAsync(cardVm.Id, result.Title, result.Description);
            if (updated is null)
                return;

            cardVm.Title = updated.Title;
            cardVm.Description = updated.Description ?? string.Empty;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
