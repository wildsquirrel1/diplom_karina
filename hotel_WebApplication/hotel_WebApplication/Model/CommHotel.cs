using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_WebApplication.Model;

public partial class CommHotel
{
    public int IdcommHotel { get; set; }

    public int IdClient { get; set; }

    public int IdHotel { get; set; }

    public int IdComm { get; set; }

    public virtual Clint IdClientNavigation { get; set; } = null!;

    //[ValidateNever]
    public virtual Comment IdCommNavigation { get; set; } = null!;
    public virtual Hotel IdHotelNavigation { get; set; } = null!;
}
