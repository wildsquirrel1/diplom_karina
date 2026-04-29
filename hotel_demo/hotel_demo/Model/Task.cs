using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Task
{
    public int Idtask { get; set; }

    public string Name { get; set; } = null!;

    public int Idroomcleaning { get; set; }

    public virtual Roomcleaningschedule IdroomcleaningNavigation { get; set; } = null!;
}
