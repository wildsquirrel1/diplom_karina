using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace hotel_WebApplication.Models;

public partial class BookService
{
    public int IdbookService { get; set; }

    public int BookId { get; set; }

    public int ServiceId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public virtual Book Book { get; set; } = null!;
    [JsonIgnore]
    [ValidateNever]
    public virtual Service Service { get; set; } = null!;
}
