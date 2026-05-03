using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace hotel_WebApplication.Model;

public partial class Book
{
    public int Idbook { get; set; }

    public int? EmployeeId { get; set; }

    public int RoomId { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly DepartureDate { get; set; }

    public DateOnly BookingDate { get; set; }

    public sbyte Payment { get; set; }

    public sbyte StatusBook { get; set; }

    public int ClientId { get; set; }

    public virtual ICollection<BookService> BookServices { get; set; } = new List<BookService>();
    /*[NotMapped]
    [JsonIgnore]*/
    [ValidateNever]
    public virtual Clint Client { get; set; } = null!;
    [ValidateNever]
    [JsonIgnore]
    public virtual Employee? Employee { get; set; }
    
    [ValidateNever]
    public virtual ICollection<GuestBook> GuestBooks { get; set; } = new List<GuestBook>();
    [ValidateNever]
    public virtual Room Room { get; set; } = null!;
}
