using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Role
{
    public int Idrole { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Employeer> Employeers { get; set; } = new List<Employeer>();
}
