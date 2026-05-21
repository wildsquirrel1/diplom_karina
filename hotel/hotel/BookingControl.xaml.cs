using hotel.Data;
using hotel.Models;
using hotel.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace hotel
{
    public partial class BookingControl : UserControl
    {
        private Book _currentBook;
        private Employee _employee;
        public event Action OnStatusChanged;

        public BookingControl()
        {
            InitializeComponent();
        }

        public void SetData(Book book, Employee employee)
        {
            _currentBook = book;
            _employee = employee;

            tbId.Text = $"Бронь №{book.Idbook}";

            string roomName = book.Room?.Name ?? "Не указан";
            tbRoom.Text = $"Номер: {roomName}";

            tbClient.Text = $"Клиент: {book.GuestFullName}";

            tbGuests.Text = book.NumOfGuests.ToString();

            tbCheckIn.Text = book.CheckInDate.ToString("dd.MM.yyyy");
            tbCheckOut.Text = book.DepartureDate.ToString("dd.MM.yyyy");

            tbTotal.Text = $"Итого: {book.TotalAmount:N0} ₽";

            tbStatus.Text = book.StatusBookName;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var canCheckIn = book.StatusBook == 4 && book.CheckInDate == today;
            var canCheckOut = book.StatusBook == 1 && book.DepartureDate == today;

            btnCheckIn.Visibility = canCheckIn ? Visibility.Visible : Visibility.Collapsed;
            btnCheckOut.Visibility = canCheckOut ? Visibility.Visible : Visibility.Collapsed;
            btnCancel.Visibility = book.StatusBook == 4 ? Visibility.Visible : Visibility.Collapsed;

            switch (book.StatusBook)
            {
                case 1:
                    statusBadge.Background = new SolidColorBrush(Color.FromRgb(46, 139, 87));
                    break;
                case 2:
                    statusBadge.Background = new SolidColorBrush(Color.FromRgb(217, 83, 79));
                    break;
                case 3:
                    statusBadge.Background = new SolidColorBrush(Color.FromRgb(119, 119, 119));
                    break;
                case 4:
                    statusBadge.Background = new SolidColorBrush(Color.FromRgb(74, 63, 222));
                    break;
                default:
                    statusBadge.Background = new SolidColorBrush(Colors.Gray);
                    break;
            }
        }

        private async void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBook == null || _employee == null) return;

            var confirm = MessageBox.Show(
                $"Заселить гостя по брони №{_currentBook.Idbook} и сформировать договор?",
                "Уведомление",
                MessageBoxButton.YesNo);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var error = await Api.UpdateBooking(_currentBook.Idbook, 1);
                if (!string.IsNullOrEmpty(error))
                {
                    MessageBox.Show($"Ошибка: {error}", "Уведомление");
                    return;
                }

                _currentBook.StatusBook = 1;

                var hotel = _employee.IdhotelNavigation ?? _currentBook.Room?.Hotel;
                var path = BookingContractPdfGenerator.GenerateAndOpen(_currentBook, _employee, hotel);

                MessageBox.Show($"Гость заселён. Договор сохранён:\n{path}", "Уведомление");
                OnStatusChanged?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка заселения: {ex.Message}", "Уведомление");
            }
        }

        private async void btnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBook == null) return;

            var confirm = MessageBox.Show(
                $"Выселить гостя по брони №{_currentBook.Idbook}?",
                "Уведомление",
                MessageBoxButton.YesNo);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var error = await Api.UpdateBooking(_currentBook.Idbook, 3);
                if (string.IsNullOrEmpty(error))
                {
                    MessageBox.Show("Бронь завершена", "Уведомление");
                    OnStatusChanged?.Invoke();
                }
                else
                    MessageBox.Show($"Ошибка: {error}", "Уведомление");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка соединения: {ex.Message}", "Уведомление");
            }
        }

        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBook == null) return;

            var result = MessageBox.Show(
                $"Отменить бронь №{_currentBook.Idbook}?",
                "Уведомление",
                MessageBoxButton.YesNo);

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
                        MessageBox.Show($"Ошибка: {error}", "Уведомление");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка соединения: {ex.Message}", "Уведомление");
                }
            }
        }
    }
}
