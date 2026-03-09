using Avalonia;
using Avalonia.Controls;

namespace ProjectPlan.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == Window.WindowStateProperty)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide(); // send window to tray instead
            }
        }
    }
}