using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Category
{
    public int Idcategory { get; set; }

    public string Name { get; set; } = null!;

    public decimal? Cost { get; set; }

    public virtual ICollection<Hoterlroom> Hoterlrooms { get; set; } = new List<Hoterlroom>();
}
