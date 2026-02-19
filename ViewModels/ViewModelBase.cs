using CommunityToolkit.Mvvm.ComponentModel;
using ProjectPlan.Models;

namespace ProjectPlan.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
  protected Context Context { get; } = new();
}
