using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectPlan.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace ProjectPlan.ViewModels;

public partial class ProjectViewModel : ViewModelBase
{
    [ObservableProperty] private int _projectId;
    [ObservableProperty] private string _title = "Project";
    [ObservableProperty] private string _hint = "Ici tu mettras le contenu de tes projets.";
    [ObservableProperty] private string _projectName = "(aucun projet sélectionné)";
    [ObservableProperty] private List<Column> _columns = new();

    partial void OnProjectIdChanged(int value)
    {
        UpdateProject();
    }

    async private void UpdateProject()
    {
        try
        {
            if (ProjectId == 0) return;
            var project = Context.Projects.FirstOrDefault(p => p.Id == ProjectId);
            if (project != null)
            {
                ProjectName = project.Name;
                Title = project.Name;
                if (!string.IsNullOrEmpty(project.Description)) Hint = project.Description;
            }

            Columns = (await Context.Projects
                .Include(p => p.Columns)
                .ThenInclude(c => c.Cards)
                .FirstAsync(p => p.Id == ProjectId))
                .Columns.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading project: {e.Message}");
        }
    }
}
