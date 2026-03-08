using System;
using System.Collections.Generic;

namespace ProjectPlan.Models;

public partial class CardHistory
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public DateTime UpdateTime { get; set; }

    public int IdCard { get; set; }

    public virtual Card IdCardNavigation { get; set; } = null!;
}
