using hotel.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace hotel.Model;

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

    public virtual Employee Employee { get; set; } = null!;
    [ValidateNever]
    public virtual Clint Client { get; set; } = null!;

    //public virtual ICollection<GuestBook> GuestBooks { get; set; } = new List<GuestBook>();
    [ValidateNever]
    public virtual Room Room { get; set; } = null!;
    //public int NumOfGuests => GuestBooks?.Count ?? 0;

    public decimal TotalAmount
    {
        get
        {
            // 1. Дни проживания
            int days = DepartureDate.DayNumber - CheckInDate.DayNumber;

            // 2. Стоимость номера
            decimal roomCost = (Room?.IdCategoryNavigation?.Cost ?? 0) * days;

            // 3. Услуги
            decimal servicesCost = BookServices?.Sum(bs => bs.Service?.Cost ?? 0) ?? 0;

            return roomCost + servicesCost;
        }
    }

    public string GuestFullName
    {
        get
        {
            if (Client == null)
                return "Клиент не указан";
            return $"{Client.Lastname} {Client.Name} {Client.Patronymic}".Trim();
        }
    }
    public string StatusBookName => StatusBook switch
    {
        1 => "Активно",
        2 => "Отменено",
        3 => "Завершено",
        _ => "Неизвестно"
    };
}
