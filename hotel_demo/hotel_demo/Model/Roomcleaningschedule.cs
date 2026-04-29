using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Roomcleaningschedule
{
    public int Idroomcleaningschedule { get; set; }

    public int Cleaningschefduleid { get; set; }

    public int Roomid { get; set; }

    public string Cleaningstatus { get; set; } = null!;

    public virtual Cleaningschedule Cleaningschefdule { get; set; } = null!;

    public virtual Hoterlroom Room { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
