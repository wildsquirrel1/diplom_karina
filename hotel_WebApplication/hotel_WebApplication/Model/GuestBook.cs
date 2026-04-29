using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace hotel_WebApplication.Model;

public partial class GuestBook
{
    public int IdguestBook { get; set; }

    public int Bookid { get; set; }

    public int GuestId { get; set; }
    [ValidateNever]
    public virtual Book Book { get; set; } = null!;
    [ValidateNever]
    public virtual Guest Guest { get; set; } = null!;
}
