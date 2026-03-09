using System;
using System.Collections.Generic;

namespace ProjectPlan.Models;

public partial class Card
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Color { get; set; }

    public bool? Notify { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int IdColumn { get; set; }

    public virtual ICollection<CardHistory> CardHistories { get; set; } = new List<CardHistory>();

    public virtual Column IdColumnNavigation { get; set; } = null!;
}
