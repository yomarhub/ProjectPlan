using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProjectPlan.ViewModels;

public sealed partial class EditProjectDialogViewModel : ViewModelBase
{
    private readonly Func<IStorageProvider?> _storageProviderGetter;

    public EditProjectDialogViewModel(
        string initialTitle,
        string initialDescription,
        string? initialImagePath,
        Func<IStorageProvider?>? storageProviderGetter = null)
    {
        _storageProviderGetter = storageProviderGetter ?? (() => null);

        _projectTitle = initialTitle;
        _projectDescription = initialDescription;
        _imagePath = initialImagePath;

        if (!string.IsNullOrWhiteSpace(_imagePath))
        {
            try
            {
                ImagePreview = new Bitmap(_imagePath);
            }
            catch
            {
                ImagePreview = null;
            }
        }
    }

    public event Action<EditProjectResult?>? RequestClose;

    [ObservableProperty]
    private string? _projectTitle;

    [ObservableProperty]
    private string? _projectDescription;

    [ObservableProperty]
    private string? _imagePath;

    [ObservableProperty]
    private IImage? _imagePreview;

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke(null);
    }

    [RelayCommand]
    private void Save()
    {
        var result = new EditProjectResult(
            (ProjectTitle ?? string.Empty).Trim(),
            (ProjectDescription ?? string.Empty).Trim(),
            ImagePath,
            ImagePreview);

        RequestClose?.Invoke(result);
    }

    [RelayCommand]
    private async Task ChooseImageAsync()
    {
        var storage = _storageProviderGetter();
        if (storage is null)
            return;

        var options = new FilePickerOpenOptions
        {
            Title = "Choisir une image",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Images")
                {
                    Patterns = ["*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp", "*.webp"],
                    MimeTypes = ["image/png", "image/jpeg", "image/gif", "image/bmp", "image/webp"],
                },
            ],
        };

        var files = await storage.OpenFilePickerAsync(options);
        var file = files?.FirstOrDefault();
        if (file is null)
            return;

        ImagePath = file.Path.LocalPath;

        try
        {
            await using var stream = await file.OpenReadAsync();
            ImagePreview = new Bitmap(stream);
        }
        catch
        {
            ImagePreview = null;
        }
    }
}

public sealed record EditProjectResult(
    string Title,
    string Description,
    string? ImagePath,
    IImage? Image);
