using hotel.Data;
using hotel.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Логика взаимодействия для AddBookingWindow.xaml
    /// </summary>
    public partial class AddBookingWindow : Window
    {
        private Employee Employee;
        private List<Room> _rooms = new();
        private List<Clint> _clients = new();
        private List<Guest> _allGuests = new();
        private List<Service> _allServices = new();
        private List<Guest> _selectedGuests = new();
        private List<Service> _selectedServices = new();
        public AddBookingWindow(Employee employee)
        {
            InitializeComponent();
            Employee = employee;
            emplName.Text += " " + employee.Name + " " + employee.Lastname;
            emplRole.Text += " " + employee.IdroleNavigation.Name;
            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var allRooms = await Api.GetRooms();
                _rooms = allRooms.Where(r => r.Hotelid == Employee.IdhotelNavigation.Idhotel).ToList();
                _clients = await Api.GetClients();
                _allGuests = await Api.GetGuests();
                
                _allServices = await Api.GetServices();

                roomCB.ItemsSource = _rooms.Select(r => r.Name).ToList();
                guestsCB.ItemsSource = _allGuests.Select(r => $"{r.Lastname} {r.Name} {r.Patronymic}").ToList();
                clientCB.ItemsSource = _clients.Select(c => $"{c.Lastname} {c.Name} {c.Patronymic}").ToList();

                servicesLB.ItemsSource = _allServices.Select(s => s.Name).ToList();
                _selectedGuests.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Уведомление");
            }
        }

        private void DateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCost();
            ValidateDateSelection();
        }

        private void ValidateDateSelection()
        {
            if (checkInDP.SelectedDate == null || checkOutDP.SelectedDate == null)
                return;

            var checkIn = checkInDP.SelectedDate.Value;
            var checkOut = checkOutDP.SelectedDate.Value;

            if (checkOut <= checkIn)
            {
                MessageBox.Show("Дата выезда должна быть позже даты заезда", "Уведомление");
                checkOutDP.SelectedDate = null;
                return;
            }

            var today = DateTime.Today.Date;
            if (checkIn.Date < today)
            {
                MessageBox.Show("Дата заезда не может быть в прошлом", "Уведомление");
                checkInDP.SelectedDate = null;
                return;
            }

            foreach (var range in checkInDP.BlackoutDates)
            {
                if (checkIn.Date >= range.Start.Date && checkIn.Date <= range.End.Date)
                {
                    MessageBox.Show($"Выбранная дата заезда недоступна (номер забронирован)", "Уведомление");
                    checkInDP.SelectedDate = null;
                    return;
                }
            }

            foreach (var range in checkOutDP.BlackoutDates)
            {
                if (checkOut.Date >= range.Start.Date && checkOut.Date <= range.End.Date)
                {
                    MessageBox.Show($"Выбранная дата выезда недоступна (номер забронирован)", "Уведомление");
                    checkOutDP.SelectedDate = null;
                    return;
                }
            }
        }
        private async void roomCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCost();

            if (roomCB.SelectedIndex != -1)
            {
                var selectedRoom = _rooms[roomCB.SelectedIndex];
                await LoadBookedDatesForRoom(selectedRoom.Idroom);
            }
        }
        private void UpdateCost()
        {
            if (roomCB.SelectedIndex == -1 || checkInDP.SelectedDate == null || checkOutDP.SelectedDate == null)
            {
                costTB.Text = "Стоимость: 0 руб.";
                return;
            }

            var room = _rooms[roomCB.SelectedIndex];
            var days = (checkOutDP.SelectedDate.Value - checkInDP.SelectedDate.Value).Days;
            if (days <= 0)
            {
                costTB.Text = "Стоимость: 0 руб.";
                return;
            }

            var roomCost = (room.IdCategoryNavigation?.Cost ?? 0) * days;
            var servicesCost = _selectedServices.Sum(s => s.Cost) * days;
            var total = roomCost + servicesCost;
            costTB.Text = $"Стоимость: {total:N0} руб.";
        }

        private void addGuest_Click(object sender, RoutedEventArgs e)
        {
            if (clientCB.SelectedIndex == -1)
            {
                MessageBox.Show("Сначала выберите клиента.", "Уведомление");
                return;
            }
            if (guestsCB.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите гостя из списка.", "Уведомление");
                return;
            }

            var selectedGuest = _allGuests[guestsCB.SelectedIndex];

            if (_selectedGuests.Any(g => g.Idguest == selectedGuest.Idguest))
            {
                MessageBox.Show("Этот гость уже добавлен в список проживающих.", "Уведомление");
                return;
            }
            _selectedGuests.Add(selectedGuest);
            guestsDG.ItemsSource = null;
            guestsDG.ItemsSource = _selectedGuests.Select(g => new
            {
                FullName = $"{g.Lastname} {g.Name} {g.Patronymic}".Trim(),

                Document = g.DocumentType == 1
            ? $"Паспорт: {g.DocumentNumber}"
            : (g.DocumentType == 2
                ? $"Св. о рождении №{g.DocumentNumber}"
                : $"Документ №{g.DocumentNumber}")
            }).ToList();
        }

        private void addService_Click(object sender, RoutedEventArgs e)
        {
            if (servicesLB.SelectedItem == null) return;

            var serviceName = servicesLB.SelectedItem.ToString();
            var service = _allServices.FirstOrDefault(s => s.Name == serviceName);
            if (service != null && !_selectedServices.Contains(service))
            {
                _selectedServices.Add(service);
                selectedServicesLB.ItemsSource = null;
                selectedServicesLB.ItemsSource = _selectedServices.Select(s => s.Name).ToList();
                UpdateCost();
            }
        }

        private async void createBooking_Click(object sender, RoutedEventArgs e)
        {
            if (roomCB.SelectedIndex == -1 || clientCB.SelectedIndex == -1 ||
                checkInDP.SelectedDate == null || checkOutDP.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля", "Уведомление");
                return;
            }

            if (checkOutDP.SelectedDate <= checkInDP.SelectedDate)
            {
                MessageBox.Show("Дата выезда должна быть позже даты заезда", "Уведомление");
                return;
            }

            var room = _rooms[roomCB.SelectedIndex];
            var client = _clients[clientCB.SelectedIndex];
            var checkIn = DateOnly.FromDateTime(checkInDP.SelectedDate.Value);
            var checkOut = DateOnly.FromDateTime(checkOutDP.SelectedDate.Value);

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (checkIn < today)
            {
                MessageBox.Show("Дата заезда не может быть в прошлом", "Уведомление");
                return;
            }

            var bookings = await Api.GetBookingsForCurrentHotel(Employee.Idemployee);

            if (bookings.Any(b => b.RoomId == room.Idroom && b.StatusBook != 4 && checkIn < b.DepartureDate && checkOut > b.CheckInDate))
            {
                MessageBox.Show(
                    "На выбранные даты номер уже забронирован!\n" +
                    "Пожалуйста, выберите другие даты или другой номер.",
                    "Уведомление");

                checkInDP.SelectedDate = null;
                checkOutDP.SelectedDate = null;
                await LoadBookedDatesForRoom(room.Idroom);
                return;
            }

            /*if (checkOutDP.SelectedDate.HasValue && checkInDP.SelectedDate.HasValue)
            {
                if (bookings.Any(b => b.RoomId == room.Idroom &&
                                      b.CheckInDate < DateOnly.FromDateTime(checkOutDP.SelectedDate.Value) &&
                                      b.DepartureDate > DateOnly.FromDateTime(checkInDP.SelectedDate.Value)))
                {
                    MessageBox.Show("Номер уже забронирован на эти даты.", "Уведомление");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите даты заезда и выезда.", "Уведомление");
                return;
            }*/

            var booking = new Book
            {
                EmployeeId = Employee.Idemployee,
                RoomId = room.Idroom,
                CheckInDate = DateOnly.FromDateTime(checkInDP.SelectedDate.Value),
                DepartureDate = DateOnly.FromDateTime(checkOutDP.SelectedDate.Value),
                BookingDate = DateOnly.FromDateTime(DateTime.Now),
                Payment = 1,
                StatusBook = 1,
                ClientId = client.Idclint,
            };

            var bookingWithId = await Api.AddBooking(booking);

            if (bookingWithId == null)
            {
                MessageBox.Show($"Ошибка при создании бронирования.", "Уведомление");
                return;
            }

            foreach (var guest in _selectedGuests)
            {
                var guestBook = new GuestBook
                {
                    Bookid = bookingWithId.Idbook,
                    GuestId = guest.Idguest
                };
                var guesterror = await Api.AddGuestToBooking(guestBook);
                if(guesterror != null)
                {
                    MessageBox.Show($"Ошибка при добавлении связи: {guesterror}", "Уведомление!");
                    return;
                }
            }

            foreach (var service in _selectedServices)
            {
                var bookService = new BookService
                {
                    ServiceId = service.Idservice,
                    BookId = bookingWithId.Idbook
                };
                var serviceError = await Api.AddServiceToBooking(bookService);
                if (serviceError != null)
                {
                    MessageBox.Show($"Ошибка при добавлении услуги: {serviceError}");
                    return;
                }
            }

            var days = (checkOutDP.SelectedDate.Value - checkInDP.SelectedDate.Value).Days;
            var roomCost = (room.IdCategoryNavigation?.Cost ?? 0) * days;
            var servicesCost = _selectedServices.Sum(s => s.Cost) * days;
            var totalCost = roomCost + servicesCost;

            GenerateConfirmationPdf(bookingWithId, room, client, totalCost, _selectedGuests, _selectedServices);

            MessageBox.Show("Бронирование успешно создано!", "Уведомление");
            DialogResult = true;
            Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void newClient_Click(object sender, RoutedEventArgs e)
        {
            var addClient = new AddClintWindow(Employee);
            if (addClient.ShowDialog() == true)
            {
                
                _ = LoadDataAsync();
            }
        }

        private void clientCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            guestsCB.ItemsSource = null;
            _selectedGuests.Clear();
            guestsDG.ItemsSource = _selectedGuests;

            if (clientCB.SelectedIndex == -1) return;

            var selectedClient = _clients[clientCB.SelectedIndex];
            var clientGuests = _allGuests.ToList();

            if (clientGuests.Any())
            {
                guestsCB.ItemsSource = clientGuests.Select(g => $"{g.Lastname} {g.Name} {g.Patronymic}".Trim()).ToList();
                guestsCB.IsEnabled = true;
            }
            else
            {
                guestsCB.IsEnabled = false;
                MessageBox.Show("У клиента нет зарегистрированных гостей.", "Уведомление");
            }
        }

        private async Task LoadBookedDatesForRoom(int roomId)
        {
            checkInDP.BlackoutDates.Clear();
            checkOutDP.BlackoutDates.Clear();

            var allBookings = await Api.GetBookingsForCurrentHotel(Employee.Idemployee);

            var roomBookings = allBookings
                .Where(b => b.RoomId == roomId && b.StatusBook != 4)
                .ToList();

            var today = DateOnly.FromDateTime(DateTime.Today);

            foreach (var booking in roomBookings)
            {
                var start = booking.CheckInDate;
                var end = booking.DepartureDate.AddDays(-1);

                if (end < today) continue;

                if (start < today) start = today;

                checkInDP.BlackoutDates.Add(new CalendarDateRange(
                    start.ToDateTime(TimeOnly.MinValue),
                    end.ToDateTime(TimeOnly.MinValue)
                ));

                checkOutDP.BlackoutDates.Add(new CalendarDateRange(
                    start.ToDateTime(TimeOnly.MinValue),
                    end.ToDateTime(TimeOnly.MinValue)
                ));
            }
        }

        private void GenerateConfirmationPdf(Book booking, Room room, Clint client, decimal totalCost, List<Guest> guests, List<Service> services)
        {
            string fileName = $"Подтверждение_брони_{booking.Idbook}_{DateTime.Now:yyyy-MM-dd}.pdf";
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            string logoPath = @"C:\Users\Пользователь\source\repos\hotel\hotel\images\local-hotel_118960.png";

            string employeeName = $"{Employee.Lastname} {Employee.Name} {Employee.Patronymic}".Trim();
            string currentDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

            try
            {
                QuestPDF.Settings.License = LicenseType.Community;

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(25, Unit.Millimetre);
                        page.DefaultTextStyle(x => x.FontSize(14).FontFamily("Arial"));

                        page.Content().Column(col =>
                        {
                            col.Item().Row(row =>
                            {
                                if (File.Exists(logoPath))
                                {
                                    row.RelativeItem(1).Height(40).AlignMiddle().Image(logoPath).FitHeight();
                                }
                                row.RelativeItem(3).PaddingLeft(20).AlignMiddle().Column(header =>
                                {
                                    header.Item().Text("Простой Комфорт").FontSize(24).Bold();
                                    header.Item().Text("Подтверждение бронирования").FontSize(18).Bold();
                                });
                            });

                            col.Item().PaddingTop(10).LineHorizontal(2).LineColor(QuestPDF.Helpers.Colors.Grey.Lighten2);

                            col.Item().PaddingTop(30).Column(details =>
                            {
                                details.Item().Text($"Статус: ПОДТВЕРЖДЕНО").FontSize(16).Bold().FontColor(QuestPDF.Helpers.Colors.Green.Medium);

                                details.Item().PaddingTop(20).Text($"Категория номера: {room.IdCategoryNavigation?.Name ?? "Стандарт"}");
                                details.Item().Text($"Номер комнаты: {room.Name}");

                                details.Item().PaddingTop(20).Text($"Основной гость (Клиент): {client.Lastname} {client.Name} {client.Patronymic}");

                                details.Item().PaddingTop(15).Column(guestCol =>
                                {
                                    if (guests != null && guests.Count > 0)
                                    {
                                        guestCol.Item().Text("Проживающие гости:").Bold().FontSize(14);

                                        foreach (var g in guests)
                                        {
                                            string fio = $"{g.Lastname} {g.Name} {g.Patronymic}".Trim();
                                            
                                            guestCol.Item().PaddingLeft(10).Text($"• {fio}");
                                        }
                                    }
                                    else
                                    {
                                        guestCol.Item().Text("Проживающие гости: Без дополнительных гостей (только основной клиент)").FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
                                    }
                                });

                                details.Item().PaddingTop(20).Text($"Дата заезда: {booking.CheckInDate:dd.MM.yyyy}");
                                details.Item().Text($"Дата выезда: {booking.DepartureDate:dd.MM.yyyy}");

                                int nights = booking.DepartureDate.DayNumber - booking.CheckInDate.DayNumber;
                                details.Item().Text($"Количество ночей: {nights}");

                                if (services != null && services.Count > 0)
                                {
                                    details.Item().PaddingTop(20).Text("Дополнительные услуги:").Bold().FontSize(14);

                                    var room = _rooms[roomCB.SelectedIndex]; // или передайте room как параметр
                                    var days = (checkOutDP.SelectedDate.Value - checkInDP.SelectedDate.Value).Days;

                                    foreach (var svc in services)
                                    {
                                        var svcTotal = svc.Cost * days;
                                        details.Item().PaddingLeft(10).Row(svcRow =>
                                        {
                                            svcRow.RelativeItem(3).Text($"• {svc.Name}");
                                            svcRow.RelativeItem(1).AlignRight().Text($"{svc.Cost:N0} ₽/день");
                                            svcRow.RelativeItem(1).AlignRight().Text($"= {svcTotal:N0} ₽").Bold();
                                        });
                                    }
                                }

                                details.Item().PaddingTop(20).Text($"Способ оплаты: {(booking.Payment == 1 ? "Карта/Онлайн" : "Наличные")}");

                                details.Item().PaddingTop(30).Row(totalRow =>
                                {
                                    totalRow.RelativeItem().Text("Итого к оплате:").FontSize(20).Bold();
                                    totalRow.RelativeItem().AlignRight().Text($"{totalCost:N0} руб.").FontSize(20).Bold();
                                });
                            });

                            col.Item().PaddingTop(60).Column(footer =>
                            {
                                footer.Item().PaddingBottom(10).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Grey.Lighten2);

                                footer.Item().Row(row =>
                                {
                                    row.RelativeItem().Column(col =>
                                    {
                                        col.Item().Text("Бронирование оформил:").Bold().FontSize(12);
                                        col.Item().Text(employeeName).FontSize(12);
                                    });

                                    row.RelativeItem().AlignRight().Column(col =>
                                    {
                                        col.Item().Text("Дата формирования:").Bold().FontSize(12).AlignRight();
                                        col.Item().Text(currentDate).FontSize(12).AlignRight();
                                    });
                                });
                            });
                        });
                    });
                }).GeneratePdf(path);

                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                MessageBox.Show($"Подтверждение создано и сохранено:\n{fileName}", "Уведомление");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании PDF: {ex.Message}", "Уведомление");
            }
        }

    }
}