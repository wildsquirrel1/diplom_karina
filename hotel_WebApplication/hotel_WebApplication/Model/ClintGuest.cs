using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace hotel_WebApplication.Model;

public partial class ClintGuest
{
    public int IdclintGuest { get; set; }

    public int GuestIt { get; set; }

    public int Clientid { get; set; }
    [JsonIgnore]
    public virtual Clint Client { get; set; } = null!;
    [JsonIgnore]
    public virtual Guest GuestItNavigation { get; set; } = null!;
}
