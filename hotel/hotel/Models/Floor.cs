using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace hotel.Models;

public partial class Floor
{
    public int Idfloor { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
