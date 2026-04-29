using System;
using System.Collections.Generic;

namespace hotel.Models;

public partial class CommHotel
{
    public int IdcommHotel { get; set; }

    public int IdClient { get; set; }

    public int IdHotel { get; set; }

    public int IdComm { get; set; }

    public virtual Clint IdClientNavigation { get; set; } = null!;

    public virtual Comment IdCommNavigation { get; set; } = null!;

    public virtual Hotel IdHotelNavigation { get; set; } = null!;
}
