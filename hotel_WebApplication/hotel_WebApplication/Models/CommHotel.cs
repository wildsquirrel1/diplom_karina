using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_WebApplication.Models;

public partial class CommHotel
{
    public int IdcommHotel { get; set; }

    public int IdClient { get; set; }

    public int IdHotel { get; set; }

    public int IdComm { get; set; }

    public virtual Clint IdClientNavigation { get; set; } = null!;

    public virtual Hotel IdHotelNavigation { get; set; } = null!;
    [NotMapped]
    public virtual Comment IdCommentNavigation { get; set; } = null!;
}
