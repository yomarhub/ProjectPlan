using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ProjectPlan.ViewModels;

namespace ProjectPlan.Views;

public partial class CreateProjectDialog : Window
{
    private readonly CreateProjectDialogViewModel _vm = new();

    public CreateProjectDialog()
    {
        InitializeComponent();
        DataContext = _vm;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void ChooseImage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var files = await PickImageAsync();
        var file = files?.FirstOrDefault();
        if (file is null)
            return;

        _vm.ImagePath = file.Path.LocalPath;

        try
        {
            await using var stream = await file.OpenReadAsync();
            _vm.ImagePreview = new Bitmap(stream);
        }
        catch
        {
            _vm.ImagePreview = null;
        }
    }

    private async Task<IReadOnlyList<IStorageFile>?> PickImageAsync()
    {
        var storage = StorageProvider;
        if (storage is null)
            return null;

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

        return await storage.OpenFilePickerAsync(options);
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(null);
    }

    private void Create_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var result = new CreateProjectResult(
            _vm.ProjectTitle ?? string.Empty,
            _vm.ProjectDescription ?? string.Empty,
            _vm.ImagePath,
            _vm.ImagePreview);

        Close(result);
    }
}
