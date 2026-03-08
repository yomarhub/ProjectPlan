using System;
using System.Collections.Generic;

namespace ProjectPlan.Models;

public partial class Column
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Color { get; set; }

    public int IdProject { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual Project IdProjectNavigation { get; set; } = null!;
}
