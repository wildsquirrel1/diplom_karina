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
using System.Windows.Shapes;

namespace hotel
{
    /// <summary>
    /// Логика взаимодействия для BookingServiceSpecialist.xaml
    /// </summary>
    public partial class BookingServiceSpecialist : Window
    {
        private Employee _Employee;

        private List<Book> _allBookings = new();
        public BookingServiceSpecialist(Employee employee)
        {
            InitializeComponent();
            _Employee = employee;
            emplName.Text += " " + employee.Name + " " + employee.Lastname;
            emplRole.Text += " " + employee.IdroleNavigation.Name;
            LoadBookings();
        }

        private async void LoadBookings()
        {

            if (bookingList == null) return;

            bookingList.Children.Clear();

            if (_allBookings.Count == 0 || string.IsNullOrEmpty(searchTB.Text))
            {
                if (_Employee?.IdhotelNavigation != null)
                {
                    _allBookings = await Api.GetBookingsForCurrentHotel(_Employee.Idemployee);

                    await AutoCompleteExpiredBookings(_allBookings);
                }
                else
                {
                    _allBookings = new List<Book>();
                }
            }

            AutoCompleteExpiredBookings(_allBookings);
            DisplayBookings(_allBookings);
        }

        private void DisplayBookings(List<Book> bookingsToShow)
        {
            bookingList.Children.Clear();

            var filteredList = ApplySearch(bookingsToShow, searchTB.Text.ToLower());
            filteredList = ApplySort(filteredList, sordCB.SelectedIndex);
            filteredList = ApplyFilter(filteredList, filterCB.SelectedIndex);

            if (filteredList == null || filteredList.Count == 0)
            {
                notfoundLabel.Visibility = Visibility.Visible;
                scrollViewer.Visibility = Visibility.Collapsed;
            }
            else
            {
                notfoundLabel.Visibility = Visibility.Collapsed;
                scrollViewer.Visibility = Visibility.Visible;

                foreach (var book in filteredList)
                {
                    var card = new BookingControl();
                    card.SetData(book);
                    card.OnStatusChanged += () => LoadBookings();
                    bookingList.Children.Add(card);
                }
            }

            NotFound();
        }

        private async Task AutoCompleteExpiredBookings(List<Book> bookings)
        {
            if (bookings == null) return;
            var today = DateOnly.FromDateTime(DateTime.Today);
            bool hasChanges = false;

            foreach (var book in bookings)
            {
                if (book.StatusBook != 3 && book.DepartureDate <= today)
                {
                    book.StatusBook = 3;
                    await Api.UpdateBooking(book.Idbook, 3);
                    hasChanges = true;
                }
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Autorisation autorisation = new Autorisation();
            autorisation.Show();
        }

        private void addBookingB_Click(object sender, RoutedEventArgs e)
        {
            var addBookingWindow = new AddBookingWindow(_Employee);
            if (addBookingWindow.ShowDialog() == true)
            {
                _allBookings.Clear();
                LoadBookings();
            }
        }

        private void NotFound()
        {
            if (bookingList.Children.Count == 0 && bookingList == null)
            {
                notfoundLabel.Visibility = Visibility.Visible;
                scrollViewer.Visibility = Visibility.Collapsed;
            }
            else
            {
                notfoundLabel.Visibility = Visibility.Collapsed;
                scrollViewer.Visibility = Visibility.Visible;
            }
        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bookingList != null)
            {
                if (!string.IsNullOrEmpty(searchTB.Text))
                    search.Visibility = Visibility.Hidden;
                else
                    search.Visibility = Visibility.Visible;

                if (string.IsNullOrEmpty(searchTB.Text) && _allBookings.Count == 0)
                {
                    LoadBookings();
                }
                else
                {
                    DisplayBookings(_allBookings);
                }
            }
        }

        private void sordCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(bookingList != null)
            {
                LoadBookings();
                NotFound();
            }
            
        }

        private void filterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bookingList != null)
            {
                LoadBookings();
                NotFound();
            }
        }

        public List<Book> ApplySearch(List<Book> booklList, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                booklList = booklList.Where(b => (b.Client.Name.ToLower().Contains(search) || b.Client.Lastname.ToLower().Contains(search) || b.Client.Patronymic.ToLower().Contains(search))).ToList();
            }
            return booklList;
        }

        private List<Book> ApplySort(List<Book> bookings, int sortIndex)
        {
            return sortIndex switch
            {
                0 => bookings.OrderBy(b => b.CheckInDate).ToList(), 
                1 => bookings.OrderByDescending(b => b.TotalAmount).ToList(),
                2 => bookings.OrderBy(b => b.TotalAmount).ToList(),
                _ => bookings.OrderBy(b => b.CheckInDate).ToList()
            };
        }

        private List<Book> ApplyFilter(List<Book> bookings, int filterIndex)
        {
            if (filterIndex == 0)
            {
                return bookings;
            }

            bookings = bookings.Where(b => b.StatusBook == filterIndex).ToList();

            return bookings;
        }

        private void clientsBtn_Click(object sender, RoutedEventArgs e)
        {
            var clientsWindow = new ClientsWindow(_Employee);
            clientsWindow.ShowDialog();

            _allBookings.Clear();
            LoadBookings();
        }
    }
}
