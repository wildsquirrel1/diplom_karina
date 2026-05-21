using hotel.Data;
using hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
    /// Логика взаимодействия для HotelsWindow.xaml
    /// </summary>
    public partial class HotelsWindow : Window
    {
        private List<Hotel> _allhotels = new();
        HeadHotelManagerWindow HeadHotelManagerWindow = new HeadHotelManagerWindow();
        private Dictionary<int, double?> _hotelRatings = new();
        Employee Employee;
        public HotelsWindow(Employee employee)
        {
            InitializeComponent();
            Employee = employee;
            HeadHotelManagerWindow.GetEmpl(employee, emplName, emplRole);
            _ = LoadHotelsAsync();
        }

        public async Task LoadHotelsAsync()
        {
            hotelList.Children.Clear();
            try
            {
                _allhotels = await Api.GetHotels();
                _hotelRatings.Clear();
                foreach (var hotel in _allhotels)
                {
                    var ratingData = await Api.GetHotelRatingAsync(hotel.Idhotel);
                    _hotelRatings[hotel.Idhotel] = ratingData?.rating;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отелей: {ex.Message}", "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var filtered = ApplySearch(_allhotels, searchTB.Text?.ToLower());
            filtered = ApplySort(filtered, sordCB.SelectedIndex);
            filtered = ApplyFilter(filtered, filterCB.SelectedIndex);

            hotelCount.Text = $"Найдено отелей: {filtered.Count}";
            foreach (var hotel in filtered)
            {
                var hotelCont = new HotelControl(Employee) { OnHotelUpdated = RefreshHotels };
                await hotelCont.DataHotel(hotel);
                hotelList.Children.Add(hotelCont);
            }

            NotFound();
        }

        private void RefreshHotels() => _ = LoadHotelsAsync();
        private void backB_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            HeadHotelManagerWindow headHotelManagerWindow = new HeadHotelManagerWindow(this.Employee);
            headHotelManagerWindow.Show();
        }

        private async void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            search.Visibility = string.IsNullOrWhiteSpace(searchTB.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
            if (hotelList != null)
            {
                await LoadHotelsAsync();
            }
        }

        private async void sordCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (hotelList != null)
            {
                await LoadHotelsAsync();
            }
        }

        private async void filterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (hotelList != null)
            {
                await LoadHotelsAsync();
            }
        }

        private async void addHotel_Click(object sender, RoutedEventArgs e)
        {
            AddHotelWindow addHotel = new AddHotelWindow(Employee);
            if (addHotel.ShowDialog() == true)
            {
                await LoadHotelsAsync();
            }

        }

        public List<Hotel> ApplySearch(List<Hotel> hotelList, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                hotelList = hotelList.Where(h => h.Name.ToLower().Contains(search)).ToList();
            }
            return hotelList;
        }

        public List<Hotel> ApplySort(List<Hotel> hotelList, int sortedIndex)
        {
            return sortedIndex switch
            {
                0 => hotelList.OrderByDescending(h => h.Idhotel).ToList(),
                1 => hotelList.OrderBy(h => h.Name).ToList(),
                2 => hotelList.OrderByDescending(h => h.Name).ToList(),
                _ => hotelList.OrderByDescending(h => h.Idhotel).ToList()
            };
        }

        public List<Hotel> ApplyFilter(List<Hotel> hotelList, int selectedValue)
        {
            if (selectedValue == 0)
            {
                return hotelList;
            }

            return hotelList.Where(h =>
            {
                // Получаем рейтинг из словаря
                if (!_hotelRatings.TryGetValue(h.Idhotel, out var rating))
                    return false;

                if (!rating.HasValue)
                    return false;

                int roundedRating = (int)Math.Round(rating.Value);
                return roundedRating == selectedValue;
            }).ToList();
        }

        public void NotFound()
        {
            if (hotelList.Children.Count == 0 || hotelList == null)
            {
                notfound.Visibility = Visibility.Visible;
                stack.Visibility = Visibility.Collapsed;
            }
            else
            {
                notfound.Visibility = Visibility.Collapsed;
                stack.Visibility = Visibility.Visible;
            }
        }
    }
}
