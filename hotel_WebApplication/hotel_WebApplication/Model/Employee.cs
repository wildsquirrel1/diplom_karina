using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace hotel_WebApplication.Model;

public partial class Employee
{
    public int Idemployee { get; set; }

    public string Name { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Email { get; set; } = null!;

    public DateOnly Birth { get; set; }

    public string Password { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int Idrole { get; set; }

    public int? Idhotel { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    [JsonIgnore]
    [ValidateNever]
    public virtual Hotel? IdhotelNavigation { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public virtual Role IdroleNavigation { get; set; } = null!;
}
