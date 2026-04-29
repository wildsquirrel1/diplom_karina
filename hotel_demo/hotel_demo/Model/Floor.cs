using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Floor
{
    public int Idfloor { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Hoterlroom> Hoterlrooms { get; set; } = new List<Hoterlroom>();
}
