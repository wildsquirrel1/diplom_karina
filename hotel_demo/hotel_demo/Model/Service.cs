using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Service
{
    public int Idservice { get; set; }

    public string Name { get; set; } = null!;

    public string Descriptiom { get; set; } = null!;

    public decimal Cost { get; set; }

    public virtual ICollection<BookService> BookServices { get; set; } = new List<BookService>();
}
