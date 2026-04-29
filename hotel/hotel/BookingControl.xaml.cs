using hotel.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hotel
{
    /// <summary>
    /// Логика взаимодействия для BookingControl.xaml
    /// </summary>
    public partial class BookingControl : UserControl
    {
        private Book _currentBook;
        public event Action OnStatusChanged;
        public BookingControl()
        {
            InitializeComponent();
        }

        public void SetData(Book book)
        {
            _currentBook = book;

            tbId.Text = $"Бронь №{book.Idbook}";

            string roomName = book.Room?.Name ?? "Не указан";
            tbRoom.Text = $"Номер: {roomName}";

            tbClient.Text = $"Клиент: {book.GuestFullName}";

            //tbGuests.Text = book.NumOfGuests.ToString();

            tbCheckIn.Text = book.CheckInDate.ToString("dd.MM.yyyy");
            tbCheckOut.Text = book.DepartureDate.ToString("dd.MM.yyyy");

            tbTotal.Text = $"Итого: {book.TotalAmount:N0} ₽";

            tbStatus.Text = book.StatusBookName;

            switch (book.StatusBook)
            {
                case 1:
                    statusBadge.Background = new SolidColorBrush(Color.FromRgb(46, 139, 87));
                    btnCancel.Visibility = Visibility.Visible;
                    break;

                case 2:
                    statusBadge.Background = new SolidColorBrush(Color.FromRgb(217, 83, 79));
                    btnCancel.Visibility = Visibility.Collapsed;
                    break;

                case 3:
                    statusBadge.Background = new SolidColorBrush(Color.FromRgb(119, 119, 119));
                    btnCancel.Visibility = Visibility.Collapsed;
                    break;

                default:
                    statusBadge.Background = new SolidColorBrush(Colors.Gray);
                    btnCancel.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBook == null) return;

            var result = MessageBox.Show(
                $"Отменить бронь №{_currentBook.Idbook}?",
                "Уведомление",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var error = await Api.UpdateBooking(_currentBook.Idbook, 2);

                    if (string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show("Бронь отменена", "Уведомление");

                        OnStatusChanged?.Invoke();
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка: {error}", "Уведомление");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка соединения: {ex.Message}", "Уведомление");
                }
            }
        }
    }
}
