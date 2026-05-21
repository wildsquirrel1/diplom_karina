using hotel.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace hotel.Services;

/// <summary>
/// PDF договора при заселении (визуально согласован с мобильным BookingContractPdfGenerator).
/// </summary>
public static class BookingContractPdfGenerator
{
    private static readonly string AccentHex = "#4A3FDE";

    private static readonly string[] RuMonths =
    {
        "", "января", "февраля", "марта", "апреля", "мая", "июня",
        "июля", "августа", "сентября", "октября", "ноября", "декабря"
    };

    public static string GenerateAndOpen(Book booking, Employee specialist, Hotel? hotel = null)
    {
        var path = Generate(booking, specialist, hotel);
        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        return path;
    }

    public static string Generate(Book booking, Employee specialist, Hotel? hotel = null)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        hotel ??= booking.Room?.Hotel ?? specialist.IdhotelNavigation;
        var hotelTitle = ResolveHotelTitle(hotel);
        var specialistFio = FormatEmployeeFio(specialist);
        var customer = booking.GuestFullName;
        var nights = Math.Max(1, booking.DepartureDate.DayNumber - booking.CheckInDate.DayNumber);
        var categoryName = booking.Room?.IdCategoryNavigation?.Name ?? "—";
        var perNight = booking.Room?.IdCategoryNavigation?.Cost ?? 0;
        var baseRub = perNight * nights;
        var serviceLines = BuildServiceLines(booking, nights);
        var servicesSum = serviceLines.Sum(l => l.Sum);
        var total = baseRub + servicesSum;

        var fileName = $"contract_{booking.Idbook}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "HotelDesktop", "pdfs");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, fileName);

        var logoBytes = TryResolveLogoBytes(hotel);
        var accent = AccentHex;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(48);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                page.Content().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        if (logoBytes != null)
                            TryAddLogo(row, logoBytes);
                        row.RelativeItem().AlignRight().Text(hotelTitle).FontSize(13).FontColor(accent);
                    });

                    col.Item().PaddingTop(20).Text($"Договор № {booking.Idbook}").FontSize(20).Bold();
                    col.Item().PaddingTop(4).LineHorizontal(1.2f).LineColor(accent);

                    col.Item().PaddingTop(16).Text("Услуги гостиниц, отелей, мотелей, кемпингов")
                        .FontSize(13).FontColor(Colors.Grey.Darken1);

                    col.Item().PaddingTop(14).Text($"Заказчик: {customer}");

                    col.Item().PaddingTop(12).Text("Период проживания").Bold();
                    col.Item().Text($"Дата заезда: {booking.CheckInDate:dd.MM.yyyy} г.").FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"Дата выезда: {booking.DepartureDate:dd.MM.yyyy} г.").FontColor(Colors.Grey.Darken1);

                    col.Item().PaddingTop(12).Text("Дополнительные услуги по брони").Bold().FontColor(accent);
                    if (serviceLines.Count == 0)
                        col.Item().PaddingLeft(10).Text("• не указаны");
                    else
                    {
                        foreach (var line in serviceLines)
                            col.Item().PaddingLeft(10).Text($"• {line.Name} ×{line.Qty} — {line.Sum:N0} ₽");
                    }

                    col.Item().PaddingTop(16).Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(28);
                            c.RelativeColumn(3);
                            c.RelativeColumn(1.2f);
                            c.RelativeColumn(1.2f);
                        });

                        table.Header(h =>
                        {
                            h.Cell().Element(HeaderCell).Text("№").Bold();
                            h.Cell().Element(HeaderCell).Text("Наименование").Bold();
                            h.Cell().Element(HeaderCell).Text("Кол-во (шт.)").Bold();
                            h.Cell().Element(HeaderCell).Text("Сумма (руб.)").Bold();
                        });

                        var row = 1;
                        table.Cell().Element(BodyCell).Text(row.ToString());
                        table.Cell().Element(BodyCell).Text(categoryName);
                        table.Cell().Element(BodyCell).Text(nights.ToString());
                        table.Cell().Element(BodyCell).Text($"{baseRub:N0}");
                        row++;

                        foreach (var line in serviceLines)
                        {
                            table.Cell().Element(BodyCell).Text(row.ToString());
                            table.Cell().Element(BodyCell).Text(line.Name);
                            table.Cell().Element(BodyCell).Text(line.Qty.ToString());
                            table.Cell().Element(BodyCell).Text($"{line.Sum:N0}");
                            row++;
                        }
                    });

                    col.Item().PaddingTop(16).Text($"Итого получено: {total:N0} ₽").FontSize(13).Bold().FontColor(accent);

                    col.Item().PaddingTop(24).Text(FormatSpecialistLine(specialistFio));
                    col.Item().PaddingTop(8).Text("Подпись: _______________________");
                    col.Item().PaddingTop(12).Text(
                        $"Дата осуществления расчёта: {FormatSettlementDatePhrase(booking.CheckInDate)}");

                    col.Item().PaddingTop(16).Text("С правилами проживания и пожарной безопасности ознакомлен:")
                        .FontSize(10.5f).FontColor(Colors.Grey.Darken1);
                    col.Item().Text("Гость: ________________ / ________________")
                        .FontSize(10.5f).FontColor(Colors.Grey.Darken1);
                });
            });
        }).GeneratePdf(path);

        return path;
    }

    private static IContainer HeaderCell(IContainer c) =>
        c.BorderBottom(1).BorderColor(AccentHex).PaddingVertical(6).PaddingHorizontal(4);

    private static IContainer BodyCell(IContainer c) =>
        c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(4);

    private sealed record ServiceLine(string Name, int Qty, decimal Sum);

    private static List<ServiceLine> BuildServiceLines(Book booking, int nights)
    {
        var list = new List<ServiceLine>();
        if (booking.BookServices == null) return list;

        foreach (var bs in booking.BookServices)
        {
            var svc = bs.Service;
            if (svc == null) continue;
            var qty = Math.Max(1, nights);
            list.Add(new ServiceLine(svc.Name, qty, svc.Cost * qty));
        }
        return list;
    }

    private static string ResolveHotelTitle(Hotel? hotel)
    {
        var n = hotel?.Name?.Trim();
        return string.IsNullOrEmpty(n) ? "Отель «Простой Комфорт»" : $"Отель «{n}»";
    }

    private static string FormatEmployeeFio(Employee e) =>
        $"{e.Lastname} {e.Name} {e.Patronymic}".Trim();

    private static string FormatSpecialistLine(string fio) =>
        string.IsNullOrWhiteSpace(fio)
            ? "Оформил специалист по бронированию (ФИО): _______________________"
            : $"Оформил специалист по бронированию (ФИО): {fio}";

    private static string FormatSettlementDatePhrase(DateOnly checkIn)
    {
        if (checkIn.Month is < 1 or > 12)
            return "«____» _____________ 20___ г.";
        return $"«{checkIn.Day}» {RuMonths[checkIn.Month]} {checkIn.Year} г.";
    }

    private static void TryAddLogo(RowDescriptor row, byte[] logoBytes)
    {
        try
        {
            // Max bounds only — FitArea scales inside without conflicting aspect-ratio constraints.
            // Content width on A4 with margin 48: ~499pt; logo cap ~72×48 leaves room for title.
            row.AutoItem().MaxWidth(72).MaxHeight(48).Image(logoBytes).FitArea();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"BookingContractPdf: logo skipped — {ex.Message}");
        }
    }

    private static byte[]? TryResolveLogoBytes(Hotel? hotel)
    {
        try
        {
            if (hotel?.Photo is { Length: > 0 })
                return hotel.Photo;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "local-hotel_118960.png");
            if (File.Exists(path))
                return File.ReadAllBytes(path);

            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "icon-icons (2).png");
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"BookingContractPdf: logo bytes unavailable — {ex.Message}");
            return null;
        }
    }
}
