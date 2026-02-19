using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ProjectPlan.ViewModels;

public sealed class DashboardCard(int id, IImage image, string title, string? description)
{
    public int Id { get; } = id;
    public IImage Image { get; } = image;
    public string Title { get; } = title;
    public string? Description { get; } = description;
}

public class DashboardViewModel : ViewModelBase
{
    public string Title { get; } = "Dashboard";

    private static IImage LoadImageOrFallback(string? assetUri, IImage? fallback = null)
    {
        fallback ??= DefaultCardImage;
        try
        {
            if (string.IsNullOrEmpty(assetUri))
                return fallback;

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

    public IReadOnlyList<DashboardCard> Cards { get; }

    public DashboardViewModel()
    {
        Cards = Context.Projects.Select(project => new DashboardCard(
            project.Id,
            LoadImageOrFallback(project.Thumbnail),
            project.Name,
            project.Description)).ToList();
    }
}