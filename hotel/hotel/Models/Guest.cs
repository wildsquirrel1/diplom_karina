using System;
using System.Collections.Generic;

namespace hotel.Models;

public partial class Guest
{
    public int Idguest { get; set; }

    public string Name { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Patronymic { get; set; } = null!;

    public sbyte DocumentType { get; set; }

    public string DocumentNumber { get; set; } = null!;

    public virtual ICollection<ClintGuest> ClintGuests { get; set; } = new List<ClintGuest>();

    public virtual ICollection<GuestBook> GuestBooks { get; set; } = new List<GuestBook>();
}
