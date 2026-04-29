using System;
using System.Collections.Generic;

namespace hotel_WebApplication.Model;

public partial class StatusRoom
{
    public int IdstatusRoom { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
