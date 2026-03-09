using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectPlan.ViewModels;

public partial class BoardCardViewModel : ViewModelBase
{
    public BoardCardViewModel(int id, int columnId, string title, string? description)
    {
        Id = id;
        _columnId = columnId;
        _title = title;
        _description = description ?? string.Empty;
    }

    public int Id { get; }

    [ObservableProperty]
    private int _columnId;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _description;
}
