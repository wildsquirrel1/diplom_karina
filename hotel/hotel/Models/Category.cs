using System;
using System.Collections.Generic;

namespace hotel.Models;

public partial class Category
{
    public int Idcategory { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Cost { get; set; }

    public int NumOfRoom { get; set; }

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<PhotoCategory> PhotoCategories { get; set; } = new List<PhotoCategory>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
