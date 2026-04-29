using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Status
{
    public int Idstatus { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Hoterlroom> Hoterlrooms { get; set; } = new List<Hoterlroom>();
}
