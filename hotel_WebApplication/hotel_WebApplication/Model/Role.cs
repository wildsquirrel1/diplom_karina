using System;
using System.Collections.Generic;

namespace hotel_WebApplication.Model;

public partial class Role
{
    public int Idrole { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
