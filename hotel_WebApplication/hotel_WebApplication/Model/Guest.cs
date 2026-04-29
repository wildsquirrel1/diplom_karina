using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace hotel_WebApplication.Model;

public partial class Guest
{
    public int Idguest { get; set; }

    public string Name { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Patronymic { get; set; } = null!;

    public sbyte DocumentType { get; set; }

    public string DocumentNumber { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<ClintGuest> ClintGuests { get; set; } = new List<ClintGuest>();
    [ValidateNever]
    public virtual ICollection<GuestBook> GuestBooks { get; set; } = new List<GuestBook>();
}
