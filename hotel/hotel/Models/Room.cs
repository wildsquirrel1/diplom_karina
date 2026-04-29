using System;
using System.Collections.Generic;

namespace hotel.Models;

public partial class Room
{
    public int Idroom { get; set; }

    public string Name { get; set; } = null!;

    public int Floorid { get; set; }

    public int IdCategory { get; set; }

    public int StatusId { get; set; }

    public int Hotelid { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual Floor Floor { get; set; } = null!;

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual Category IdCategoryNavigation { get; set; } = null!;

    public virtual StatusRoom Status { get; set; } = null!;
}
