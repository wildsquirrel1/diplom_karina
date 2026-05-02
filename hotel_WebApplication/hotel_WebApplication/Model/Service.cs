using System;
using System.Collections.Generic;

namespace hotel_WebApplication.Model;

public partial class Service
{
    public int Idservice { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Cost { get; set; }
    public sbyte? Status { get; set; }

    public virtual ICollection<BookService> BookServices { get; set; } = new List<BookService>();
}
