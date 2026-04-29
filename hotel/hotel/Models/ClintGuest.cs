using System;
using System.Collections.Generic;

namespace hotel.Models;

public partial class ClintGuest
{
    public int IdclintGuest { get; set; }

    public int GuestIt { get; set; }

    public int Clientid { get; set; }

    public virtual Clint Client { get; set; } = null!;

    public virtual Guest GuestItNavigation { get; set; } = null!;
}
