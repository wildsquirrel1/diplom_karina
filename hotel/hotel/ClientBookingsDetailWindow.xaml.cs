using hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hotel
{
    /// <summary>
    /// Логика взаимодействия для ClientBookingsDetailWindow.xaml
    /// </summary>
    public partial class ClientBookingsDetailWindow : Window
    {
        Employee _employee;

        private List<Book> _allBookings;
        private Clint _currentClient;

        private static Brush AppBrush(string key, Brush fallback)
        {
            if (Application.Current?.TryFindResource(key) is Brush b)
                return b;
            return fallback;
        }
        public ClientBookingsDetailWindow(Clint client, List<Book> clientBookings, Employee employee)
        {
            InitializeComponent();
            _employee = employee;

            _currentClient = client;
            _allBookings = new List<Book>(clientBookings);

            emplName.Text += $" {employee.Name} {employee.Lastname}";
            emplRole.Text += " " + employee.IdroleNavigation.Name;

            clientHeader.Text = $"Бронирования: {client.Lastname} {client.Name} {client.Patronymic}";

            DisplayBookings(_allBookings);
        }

        private void DisplayBookings(List<Book> allBookings)
        {
            bookingsList.Children.Clear();

            var textPrimary = AppBrush("Brush.TextPrimary", Brushes.Black);
            var textSecondary = AppBrush("Brush.TextSecondary", Brushes.DimGray);
            var textMuted = AppBrush("Brush.TextMuted", Brushes.Gray);
            var borderSubtle = AppBrush("Brush.BorderSubtle", Brushes.LightGray);
            var surface = AppBrush("Brush.Surface", Brushes.White);

            if (allBookings.Count == 0)
            {
                bookingsList.Children.Add(new TextBlock
                {
                    Text = "У клиента пока нет бронирований",
                    Foreground = textMuted,
                    FontSize = 18,
                    Margin = new Thickness(12, 8, 12, 8)
                });
                return;
            }

            foreach (var booking in allBookings)
            {
                var card = new Border
                {
                    BorderBrush = borderSubtle,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(14),
                    Margin = new Thickness(8, 0, 8, 16),
                    Padding = new Thickness(20, 18, 20, 18),
                    Background = surface
                };

                var stack = new StackPanel();

                stack.Children.Add(new TextBlock
                {
                    Text = $"Номер: {booking.Room?.Name ?? "Не указан"}",
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 22,
                    Foreground = textPrimary,
                    TextWrapping = TextWrapping.Wrap
                });
                stack.Children.Add(new TextBlock
                {
                    Text = $"{booking.CheckInDate:dd.MM.yyyy} — {booking.DepartureDate:dd.MM.yyyy}",
                    FontSize = 18,
                    Foreground = textSecondary,
                    Margin = new Thickness(0, 8, 0, 0),
                    TextWrapping = TextWrapping.Wrap
                });
                stack.Children.Add(new TextBlock
                {
                    Text = $"Статус: {GetStatusName(booking.StatusBook)}",
                    FontSize = 17,
                    Foreground = textSecondary,
                    Margin = new Thickness(0, 6, 0, 0),
                    TextWrapping = TextWrapping.Wrap
                });
                stack.Children.Add(new TextBlock
                {
                    Text = $"Дата бронирования: {booking.BookingDate:dd.MM.yyyy}",
                    FontSize = 15,
                    Foreground = textMuted,
                    Margin = new Thickness(0, 10, 0, 12),
                    TextWrapping = TextWrapping.Wrap
                });

                stack.Children.Add(new Separator
                {
                    Background = borderSubtle,
                    Height = 1,
                    Margin = new Thickness(0, 4, 0, 12)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = "Проживающие",
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 17,
                    Foreground = textPrimary,
                    Margin = new Thickness(0, 0, 0, 6)
                });


                if (booking.GuestBooks != null && booking.GuestBooks.Any())
                {
                    foreach (var guestBook in booking.GuestBooks)
                    {
                        var guest = guestBook.Guest;
                        if (guest != null)
                        {
                            var guestText = new TextBlock
                            {
                                Text = $"{guest.Lastname} {guest.Name} {guest.Patronymic} | " +
                                       $"{GetDocumentType(guest.DocumentType)}: {guest.DocumentNumber}",
                                FontSize = 16,
                                Foreground = textSecondary,
                                Margin = new Thickness(4, 2, 0, 2),
                                TextWrapping = TextWrapping.Wrap
                            };
                            stack.Children.Add(guestText);
                        }
                    }
                }
                else
                {
                    stack.Children.Add(new TextBlock
                    {
                        Text = "Без дополнительных гостей (только основной клиент)",
                        FontSize = 15,
                        Foreground = textMuted,
                        FontStyle = FontStyles.Italic,
                        TextWrapping = TextWrapping.Wrap
                    });
                }

                stack.Children.Add(new Separator
                {
                    Background = borderSubtle,
                    Height = 1,
                    Margin = new Thickness(0, 4, 0, 12)
                });

                if (booking.BookServices != null && booking.BookServices.Any())
                {
                    stack.Children.Add(new TextBlock
                    {
                        Text = "Дополнительные услуги",
                        FontWeight = FontWeights.SemiBold,
                        FontSize = 17,
                        Foreground = textPrimary,
                        Margin = new Thickness(0, 6, 0, 6)
                    });

                    var groupedServices = booking.BookServices
                        .GroupBy(bs => bs.ServiceId)
                        .Select(g => new
                        {
                            Service = g.First().Service,
                            Quantity = g.Count(),
                            TotalCost = g.Sum(bs => bs.Service?.Cost ?? 0)
                        });

                    //decimal servicesTotal = 0;

                    foreach (var item in groupedServices)
                    {
                        var serviceName = item.Service?.Name ?? "Услуга";
                        var cost = item.Service?.Cost ?? 0;
                        var quantity = item.Quantity;
                        var total = item.TotalCost;

                        //servicesTotal += total;

                        var quantityText = quantity > 1 ? $" × {quantity}" : "";

                        stack.Children.Add(new TextBlock
                        {
                            Text = $"• {serviceName}{quantityText} — {total:N0} ₽",
                            FontSize = 16,
                            Foreground = textSecondary,
                            Margin = new Thickness(8, 2, 0, 0),
                            TextWrapping = TextWrapping.Wrap
                        });
                    }
                }

                var roomCostPerNight = booking.Room?.IdCategoryNavigation?.Cost ?? 0;
                var nights = booking.DepartureDate.DayNumber - booking.CheckInDate.DayNumber;
                var roomTotal = roomCostPerNight * nights;

                var servicesTotal = booking.BookServices?
                    .GroupBy(bs => bs.ServiceId)
                    .Sum(g => g.Sum(bs => bs.Service?.Cost ?? 0)) ?? 0;

                var grandTotal = roomTotal + servicesTotal;

                stack.Children.Add(new Separator
                {
                    Height = 1,
                    Margin = new Thickness(0, 15, 0, 10)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Проживание ({nights} ночей × {roomCostPerNight:N0} ₽): {roomTotal:N0} ₽",
                    FontSize = 16,
                    Foreground = textSecondary,
                    Margin = new Thickness(0, 0, 0, 4),
                    TextWrapping = TextWrapping.Wrap
                });

                if (servicesTotal > 0)
                {
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"Услуги: {servicesTotal:N0} ₽",
                        FontSize = 16,
                        Foreground = textSecondary,
                        Margin = new Thickness(0, 0, 0, 4),
                        TextWrapping = TextWrapping.Wrap
                    });
                }

                stack.Children.Add(new TextBlock
                {
                    Text = $"Итого: {grandTotal:N0} ₽",
                    FontSize = 22,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = textPrimary,
                    Margin = new Thickness(0, 12, 0, 0),
                    TextWrapping = TextWrapping.Wrap
                });

                card.Child = stack;
                bookingsList.Children.Add(card);
            }
        }

        private string GetDocumentType(sbyte type) => type switch
        {
            1 => "Паспорт",
            2 => "Св. о рождении",
            _ => "Документ"
        };

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string GetStatusName(sbyte status) => status switch
        {
            1 => "Активно",
            2 => "Отменено",
            3 => "Завершено",
            4 => "Предстоящая",
            _ => "Неизвестно"
        };

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            search.Visibility = string.IsNullOrWhiteSpace(searchTB.Text)
                ? Visibility.Visible
                : Visibility.Hidden;

            // Поиск по дате
            var searchText = searchTB.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                DisplayBookings(_allBookings);
                return;
            }

            if (DateOnly.TryParse(searchText, new System.Globalization.CultureInfo("ru-RU"), System.Globalization.DateTimeStyles.None, out var searchDate))
            {
                var filtered = _allBookings
                    .Where(b => b.CheckInDate == searchDate)
                    .ToList();
                DisplayBookings(filtered);
            }
            else
            {
                bookingsList.Children.Clear();
                bookingsList.Children.Add(new TextBlock
                {
                    Text = "Введите дату в формате ДД.ММ.ГГГГ",
                    Foreground = AppBrush("Brush.Danger", Brushes.DarkOrange),
                    FontSize = 16,
                    Margin = new Thickness(12, 8, 12, 8),
                    TextWrapping = TextWrapping.Wrap
                });
            }
        }
    }
}
