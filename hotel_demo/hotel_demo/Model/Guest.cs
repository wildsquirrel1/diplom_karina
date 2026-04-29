using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Guest
{
    public int Idguest { get; set; }

    public string Lastname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateOnly Birth { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string Passport { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
