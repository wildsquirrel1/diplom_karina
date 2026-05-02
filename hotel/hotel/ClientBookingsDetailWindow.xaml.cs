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
        public ClientBookingsDetailWindow(Clint client, List<Book> clientBookings, Employee employee)
        {
            InitializeComponent();
            _employee = employee;

            emplName.Text += $" {employee.Name} {employee.Lastname}";
            emplRole.Text += " " + employee.IdroleNavigation.Name;

            clientHeader.Text = $"Бронирования: {client.Lastname} {client.Name} {client.Patronymic}";

            if (clientBookings.Count == 0)
            {
                bookingsList.Children.Add(new TextBlock
                {
                    Text = "У клиента пока нет бронирований",
                    Foreground = Brushes.Gray,
                    FontSize = 18,
                    Margin = new Thickness(10)
                });
                return;
            }

            foreach (var booking in clientBookings)
            {
                var card = new Border
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Margin = new Thickness(0, 0, 0, 15),
                    Padding = new Thickness(12),
                    Background = Brushes.White
                };

                var stack = new StackPanel();

                stack.Children.Add(new TextBlock
                {
                    Text = $"Номер: {booking.Room?.Name ?? "Не указан"}",
                    FontWeight = FontWeights.Bold,
                    FontSize = 20
                });
                stack.Children.Add(new TextBlock
                {
                    Text = $"{booking.CheckInDate:dd.MM.yyyy} — {booking.DepartureDate:dd.MM.yyyy}",
                    FontSize = 18,
                    Margin = new Thickness(0, 5, 0, 0)
                });
                stack.Children.Add(new TextBlock
                {
                    Text = $"Статус: {GetStatusName(booking.StatusBook)}",
                    FontSize = 18,
                    Margin = new Thickness(0, 5, 0, 10)
                });

                stack.Children.Add(new Separator
                {
                    Background = Brushes.LightGray,
                    Height = 1,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = "Проживающие:",
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    Margin = new Thickness(0, 0, 0, 5)
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
                                Margin = new Thickness(10, 2, 0, 2),
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
                        Text = "  Без дополнительных гостей (только основной клиент)",
                        FontSize = 16,
                        Foreground = Brushes.Gray,
                        FontStyle = FontStyles.Italic
                    });
                }

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
            _ => "Неизвестно"
        };
    }
}
