using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Cleaningschedule
{
    public int Idschedule { get; set; }

    public int Staffid { get; set; }

    public string Dayofweek { get; set; } = null!;

    public string Month { get; set; } = null!;

    public virtual ICollection<Roomcleaningschedule> Roomcleaningschedules { get; set; } = new List<Roomcleaningschedule>();

    public virtual Staff Staff { get; set; } = null!;
}
