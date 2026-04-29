using System;
using System.Collections.Generic;

namespace hotel.Models;

public partial class Favorite
{
    public int Idfavorite { get; set; }

    public int Idclint { get; set; }

    public int Idcategory { get; set; }

    public virtual Category IdcategoryNavigation { get; set; } = null!;

    public virtual Clint IdclintNavigation { get; set; } = null!;
}
