using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace hotel_WebApplication.Models;

public partial class Clint
{
    public int Idclint { get; set; }

    public string Name { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Patronymic { get; set; } = null!;

    public string SeriaPass { get; set; } = null!;

    public string NumberPass { get; set; } = null!;

    public DateOnly Birth { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? Photo { get; set; }

    [ValidateNever]
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual ICollection<ClintGuest> ClintGuests { get; set; } = new List<ClintGuest>();

    public virtual ICollection<CommHotel> CommHotels { get; set; } = new List<CommHotel>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}
