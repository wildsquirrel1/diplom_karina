using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace hotel_WebApplication.Models;

public partial class Room
{
    public int Idroom { get; set; }

    public string Name { get; set; } = null!;

    public int Floorid { get; set; }

    public int IdCategory { get; set; }

    public int StatusId { get; set; }

    public int Hotelid { get; set; }
    [ValidateNever]
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    [ValidateNever]
    public virtual Floor Floor { get; set; } = null!;
    [ValidateNever]
    public virtual Hotel Hotel { get; set; } = null!;
    [ValidateNever]
    public virtual Category IdCategoryNavigation { get; set; } = null!;
    [ValidateNever]
    public virtual StatusRoom Status { get; set; } = null!;
}
