using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Hoterlroom
{
    public int Idroom { get; set; }

    public string Name { get; set; } = null!;

    public decimal Cost { get; set; }

    public int Statusid { get; set; }

    public int Categoryid { get; set; }

    public int Floorid { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual Category Category { get; set; } = null!;

    public virtual Floor Floor { get; set; } = null!;

    public virtual ICollection<Roomcleaningschedule> Roomcleaningschedules { get; set; } = new List<Roomcleaningschedule>();

    public virtual Status Status { get; set; } = null!;
}
