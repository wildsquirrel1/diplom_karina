using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Staff
{
    public int Idstaff { get; set; }

    public string Lastname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateOnly Birth { get; set; }

    public virtual ICollection<Cleaningschedule> Cleaningschedules { get; set; } = new List<Cleaningschedule>();
}
