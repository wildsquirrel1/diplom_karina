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
        public static HotelsWindow Instance { get; private set; }
        private List<Hotel> _allhotels = new();
        private string _currentSearch = "";
        private int _currentSortIndex = 0;
        HeadHotelManagerWindow HeadHotelManagerWindow = new HeadHotelManagerWindow();
        private Dictionary<int, double?> _hotelRatings = new();
        Employee Employee;
        public HotelsWindow(Employee employee)
        {
            InitializeComponent();
            loadHotels();
            Instance = this;
            Employee = employee;
            HeadHotelManagerWindow.GetEmpl(employee, emplName, emplRole);
        }

        public async void loadHotels()
        {
            hotelList.Children.Clear();
            if (_allhotels.Count == 0 || string.IsNullOrEmpty(_currentSearch))
            {
                _allhotels = await Api.GetHotels();
                _hotelRatings.Clear();
                foreach (var hotel in _allhotels)
                {
                    var ratingData = await Api.GetHotelRatingAsync(hotel.Idhotel);
                    _hotelRatings[hotel.Idhotel] = ratingData?.rating;
                }
            }

            _allhotels = ApplySearch(_allhotels, searchTB.Text.ToLower());
            _allhotels = ApplySort(_allhotels, sordCB.SelectedIndex);
            _allhotels = ApplyFilter(_allhotels, filterCB.SelectedIndex);

            hotelCount.Text = $"Найдено отелей: {_allhotels.Count}";
            if (_allhotels == null || _allhotels.Count == 0)
            {
                notfound.Visibility = Visibility.Visible;
                stack.Visibility = Visibility.Collapsed;
            }
            else
            {
                foreach (var hotel in _allhotels)
                {
                    var hotelCont = new HotelControl(Employee);
                    hotelCont.DataHotel(hotel);
                    hotelList.Children.Add(hotelCont);
                }

                NotFound();
            }
        }
        private void backB_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            HeadHotelManagerWindow headHotelManagerWindow = new HeadHotelManagerWindow(this.Employee);
            headHotelManagerWindow.Show();
        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            search.Visibility = string.IsNullOrWhiteSpace(searchTB.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
            if (hotelList != null)
            {
                loadHotels();
                NotFound();
            }
        }

        private void sordCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (hotelList != null)
            {
                loadHotels();
                NotFound();
            }
        }

        private void filterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (hotelList != null)
            {
                loadHotels();
                NotFound();
            }
        }

        private void addHotel_Click(object sender, RoutedEventArgs e)
        {
            AddHotelWindow addHotel = new AddHotelWindow(Employee);
            if (addHotel.ShowDialog() == true)
            {
                _allhotels.Clear();
                _hotelRatings.Clear();
                loadHotels();
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
