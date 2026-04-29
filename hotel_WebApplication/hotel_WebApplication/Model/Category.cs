using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_WebApplication.Model;

public partial class Category
{
    public int Idcategory { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Cost { get; set; }

    public int NumOfRoom { get; set; }
    [NotMapped]
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    [NotMapped]
    public virtual ICollection<PhotoCategory> PhotoCategories { get; set; } = new List<PhotoCategory>();
    [NotMapped]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
