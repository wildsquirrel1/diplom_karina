using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace hotel_WebApplication.Model;

public partial class Service
{
    public int Idservice { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Cost { get; set; }
    public sbyte? Status { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public virtual ICollection<BookService> BookServices { get; set; } = new List<BookService>();
}
