using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using ProjectPlan.Infrastructure;
using ProjectPlan.Models;
using ProjectPlan.Services;
using ProjectPlan.Views;

namespace ProjectPlan.ViewModels;

public interface IDashboardTile
{
}

public sealed class DashboardProjectCard : IDashboardTile
{
    public DashboardProjectCard(int projectId, IImage image, string title, string description)
    {
        ProjectId = projectId;
        Image = image;
        Title = title;
        Description = description;
    }

    public int ProjectId { get; }
    public IImage Image { get; }
    public string Title { get; }
    public string Description { get; }
}

public sealed class DashboardAddCard : IDashboardTile
{
}

public partial class DashboardViewModel : ViewModelBase
{
    public string Title { get; } = "Dashboard";

    private static Window? GetMainWindow()
    {
        return (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
    }

    private static IImage LoadImageOrFallback(string assetUri, IImage fallback)
    {
        try
        {
            var uri = new Uri(assetUri);
            if (!AssetLoader.Exists(uri))
                return fallback;

            return new Bitmap(AssetLoader.Open(uri));
        }
        catch (FileNotFoundException)
        {
            return fallback;
        }
        catch (Exception)
        {
            return fallback;
        }
    }

    private static readonly IImage DefaultCardImage = new Bitmap(
        AssetLoader.Open(AssetUri.For("Assets/Default.png")));

    private static readonly IImage ThalesCardImage = LoadImageOrFallback(
        AssetUri.For("Assets/Thales.png").ToString(),
        DefaultCardImage);

    private static readonly IImage DockerCardImage = LoadImageOrFallback(
        AssetUri.For("Assets/Docker.png").ToString(),
        DefaultCardImage);

    public DashboardViewModel()
    {
        // Seed UI with a few sample cards if DB is empty (removed on first refresh).
        Tiles =
        [
            new DashboardProjectCard(
                -1,
                DefaultCardImage,
                "Avalonia Test",
                "Avalonia est un framework UI multiplateforme pour .NET, inspiré de WPF. Il te permet de créer des applications modernes et performantes pour Windows, Linux et macOS."),
            new DashboardProjectCard(
                -2,
                ThalesCardImage,
                "Thales Test",
                "Thales est une entreprise française spécialisée dans l'aérospatiale, la défense, la sécurité et le transport terrestre. Elle conçoit et fabrique des systèmes et des équipements pour les marchés civils et militaires."),
            new DashboardProjectCard(
                -3,
                DockerCardImage,
                "Docker Test",
                "Docker est une plateforme de conteneurisation qui permet aux développeurs de créer, déployer et exécuter des applications dans des conteneurs légers et portables. Docker facilite la gestion des dépendances et la distribution des applications."),
            new DashboardAddCard(),
        ];
    }

    public ObservableCollection<IDashboardTile> Tiles { get; }

    private static IImage LoadImageFromFileOrFallback(string? filePath, IImage fallback)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return fallback;

        try
        {
            if (!File.Exists(filePath))
                return fallback;

            return new Bitmap(filePath);
        }
        catch
        {
            return fallback;
        }
    }

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        await Context.EnsureDatabaseCreated();

        var projects = await DataFunctions.GetProjectsAsync(cancellationToken);

        Tiles.Clear();

        foreach (var project in projects)
        {
            Tiles.Add(new DashboardProjectCard(
                project.Id,
                LoadImageFromFileOrFallback(project.Thumbnail, DefaultCardImage),
                project.Name,
                project.Description ?? string.Empty));
        }

        Tiles.Add(new DashboardAddCard());
    }

    [RelayCommand]
    private async Task AddProjectAsync()
    {
        var window = GetMainWindow();
        if (window?.DataContext is not MainWindowViewModel mainVm)
            return;

        var dialog = new CreateProjectDialog
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        var result = await dialog.ShowDialog<CreateProjectResult?>(window);
        if (result is null)
            return;

        await mainVm.CreateNewProjectAsync(result);
    }
}
