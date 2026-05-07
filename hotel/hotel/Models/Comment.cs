using System;
using System.Collections.Generic;

namespace hotel.Models;

public partial class Comment
{
    public int Idcomment { get; set; }

    public string Comment1 { get; set; } = null!;

    public int Stars { get; set; }

    public DateOnly? Date { get; set; }
    public sbyte? Status { get; set; }

    public virtual ICollection<CommHotel> CommHotels { get; set; } = new List<CommHotel>();
}
