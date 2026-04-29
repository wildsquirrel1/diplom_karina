using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class BookService
{
    public int IdbookService { get; set; }

    public int Serviceid { get; set; }

    public int Bookid { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
