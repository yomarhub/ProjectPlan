using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ProjectPlan.Views;

public partial class ConfirmDeleteProjectDialog : Window
{
    public ConfirmDeleteProjectDialog()
    {
        InitializeComponent();
    }

    public ConfirmDeleteProjectDialog(string projectName) : this()
    {
        var message = this.FindControl<TextBlock>("MessageText");
        if (message is not null)
        {
            message.Text =
                $"Supprimer le projet \"{projectName}\" ?\n\n" +
                "Cette action supprimera aussi son contenu (colonnes et tickets).";
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(false);
    }

    private void Delete_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(true);
    }
}
