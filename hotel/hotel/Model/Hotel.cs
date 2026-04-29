using System;
using System.Collections.Generic;

namespace hotel.Model;

public partial class Hotel
{
    public int Idhotel { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? City { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? Stars { get; set; }

    public byte[]? Photo { get; set; }

    public virtual ICollection<CommHotel> CommHotels { get; set; } = new List<CommHotel>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
