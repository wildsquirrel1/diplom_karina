using System;
using System.Collections.Generic;

namespace hotel.Model;

public partial class Photo
{
    public int Idphoto { get; set; }

    public byte[] Photo1 { get; set; } = null!;

    public virtual ICollection<PhotoCategory> PhotoCategories { get; set; } = new List<PhotoCategory>();
}
