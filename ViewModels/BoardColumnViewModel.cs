using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectPlan.ViewModels;

public partial class BoardColumnViewModel : ViewModelBase
{
    public BoardColumnViewModel(int id, string name)
    {
        Id = id;
        _name = name;
    }

    public int Id { get; }

    [ObservableProperty]
    private string _name;

    public ObservableCollection<BoardCardViewModel> Cards { get; } = new();
}
