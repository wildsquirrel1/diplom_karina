using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Book
{
    public int Bookid { get; set; }

    public int Guestid { get; set; }

    public int Roomid { get; set; }

    public DateTime Dateofentry { get; set; }

    public DateTime? Departuredate { get; set; }

    public DateOnly Bookingdate { get; set; }

    public bool? Payment { get; set; }

    public string Cardstatus { get; set; } = null!;

    public int Employeerid { get; set; }

    public virtual ICollection<BookService> BookServices { get; set; } = new List<BookService>();

    public virtual Employeer Employeer { get; set; } = null!;

    public virtual Guest Guest { get; set; } = null!;

    public virtual Hoterlroom Room { get; set; } = null!;
}
