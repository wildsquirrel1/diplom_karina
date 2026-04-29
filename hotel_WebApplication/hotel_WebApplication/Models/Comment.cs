using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_WebApplication.Models;

public partial class Comment
{
    public int Idcomment { get; set; }

    public string Comment1 { get; set; } = null!;

    public int Stars { get; set; }

    public DateOnly? Date { get; set; }
    [NotMapped]
    public virtual ICollection<CommHotel> CommHotels { get; set; } = new List<CommHotel>();
}
