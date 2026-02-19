using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ProjectPlan.ViewModels;

public abstract class DashboardTileBase
{
}

public sealed class DashboardProjectCard : DashboardTileBase
{
    public DashboardProjectCard(IImage image, string title, string description)
    {
        Image = image;
        Title = title;
        Description = description;
    }

    public IImage Image { get; }
    public string Title { get; }
    public string Description { get; }
}

public sealed class DashboardAddCard : DashboardTileBase
{
}

public partial class DashboardViewModel : ViewModelBase
{
    public string Title { get; } = "Dashboard";

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
        AssetLoader.Open(new Uri("avares://ProjectPlan/Assets/avalonia-logo.ico")));

    private static readonly IImage ThalesCardImage = LoadImageOrFallback(
        "avares://ProjectPlan/Assets/Thales.png",
        DefaultCardImage);

    private static readonly IImage DockerCardImage = LoadImageOrFallback(
        "avares://ProjectPlan/Assets/Docker.png",
        DefaultCardImage);

    public IReadOnlyList<DashboardTileBase> Tiles { get; } =
    [
        new DashboardProjectCard(
            DefaultCardImage,
            "Avalonia Test",
            "Avalonia est un framework UI multiplateforme pour .NET, inspiré de WPF. Il te permet de créer des applications modernes et performantes pour Windows, Linux et macOS."),
        new DashboardProjectCard(
            ThalesCardImage,
            "Thales Test",
            "Thales est une entreprise française spécialisée dans l'aérospatiale, la défense, la sécurité et le transport terrestre. Elle conçoit et fabrique des systèmes et des équipements pour les marchés civils et militaires."),
        new DashboardProjectCard(
            DockerCardImage,
            "Docker Test",
            "Docker est une plateforme de conteneurisation qui permet aux développeurs de créer, déployer et exécuter des applications dans des conteneurs légers et portables. Docker facilite la gestion des dépendances et la distribution des applications."),
        new DashboardAddCard(),
    ];
}
