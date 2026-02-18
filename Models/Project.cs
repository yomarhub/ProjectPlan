using System;
using System.Collections.Generic;

namespace ProjectPlan.Models;

public partial class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Thumbnail { get; set; }

    public string? Background { get; set; }

    public DateTime? CreationDate { get; set; }

    public bool? Mute { get; set; }

    public virtual ICollection<Column> Columns { get; set; } = new List<Column>();
}
