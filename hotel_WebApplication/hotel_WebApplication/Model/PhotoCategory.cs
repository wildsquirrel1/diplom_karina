using System;
using System.Collections.Generic;

namespace hotel_WebApplication.Model;

public partial class PhotoCategory
{
    public int IdphotoCategory { get; set; }

    public int CategoryId { get; set; }

    public int PhotoId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Photo Photo { get; set; } = null!;
}
