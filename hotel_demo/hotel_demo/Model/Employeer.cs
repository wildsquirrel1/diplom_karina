using System;
using System.Collections.Generic;

namespace hotel_demo.Model;

public partial class Employeer
{
    public int Idemployeer { get; set; }

    public string Lastname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateOnly Birth { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Roleid { get; set; }

    public bool Isblocked { get; set; }

    public DateOnly? LastEntry { get; set; }

    public int? Entrycount { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual Role Role { get; set; } = null!;
}
