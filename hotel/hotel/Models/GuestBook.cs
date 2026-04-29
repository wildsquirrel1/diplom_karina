using System;
using System.Collections.Generic;

namespace hotel.Models;

public partial class GuestBook
{
    public int IdguestBook { get; set; }

    public int Bookid { get; set; }

    public int GuestId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Guest Guest { get; set; } = null!;
}
