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
    /// Логика взаимодействия для ReviewWindow.xaml
    /// </summary>
    public partial class ReviewWindow : Window
    {
        private readonly int _hotelId;
        private Employee _employee;
        private List<dynamic> _allReviews = new();
        private string _currentSearch = "";
        private int _currentSortIndex = 0;
        private int _currentFilterIndex = 0;

        public ReviewWindow(int hotelId, Employee employee)
        {
            InitializeComponent();
            _hotelId = hotelId;
            _employee = employee;
            emplName.Text += " " + employee.Name + " " + employee.Lastname;
            emplRole.Text += " " + employee.IdroleNavigation.Name;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadReviews();
        }

        private async Task LoadReviews()
        {
            if (ReviewsPanel == null) return;

            ReviewsPanel.Children.Clear();

            if (_allReviews.Count == 0 || string.IsNullOrEmpty(searchTB.Text))
            {
                _allReviews = await Api.GetHotelReviewsAsync(_hotelId);
            }

            DisplayReviews(_allReviews);
        }

        private void DisplayReviews(List<dynamic> reviewsToShow)
        {
            ReviewsPanel.Children.Clear();

            var filteredList = ApplySearch(reviewsToShow, searchTB.Text.ToLower());
            filteredList = ApplySort(filteredList, sordCB.SelectedIndex);
            filteredList = ApplyFilter(filteredList, filterCB.SelectedIndex);

            if (filteredList == null || filteredList.Count == 0)
            {
                notfound.Visibility = Visibility.Visible;
                stack.Visibility = Visibility.Collapsed;
            }
            else
            {
                notfound.Visibility = Visibility.Collapsed;
                stack.Visibility = Visibility.Visible;

                foreach (var review in filteredList)
                {
                    string text = review?.text?.ToString() ?? "Без текста";
                    string clientName = review?.clientName?.ToString() ?? "Аноним";
                    int stars = review?.stars != null ? (int)review.stars : 0;
                    DateTime date = review?.date != null ? (DateTime)review.date : DateTime.Now;

                    var reviewControl = new ReviewControl();
                    reviewControl.SetData(text, clientName, stars, date);
                    ReviewsPanel.Children.Add(reviewControl);
                }
            }

            NotFound();
        }

        private List<dynamic> ApplySearch(List<dynamic> reviews, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                reviews = reviews.Where(r =>
                    (r?.clientName?.ToString()?.ToLower().Contains(search) == true) ||
                    (r?.text?.ToString()?.ToLower().Contains(search) == true)
                ).ToList();
            }
            return reviews;
        }

        private List<dynamic> ApplySort(List<dynamic> reviews, int sortIndex)
        {
            return sortIndex switch
            {
                0 => reviews.OrderBy(r => r?.date).ToList(),              // По дате (сначала старые)
                1 => reviews.OrderByDescending(r => r?.date).ToList(),    // По дате (сначала новые)
                2 => reviews.OrderByDescending(r => r?.stars).ToList(),   // По рейтингу (высокий → низкий)
                3 => reviews.OrderBy(r => r?.stars).ToList(),             // По рейтингу (низкий → высокий)
                _ => reviews.OrderByDescending(r => r?.date).ToList()     // По умолчанию: новые сверху
            };
        }

        private List<dynamic> ApplyFilter(List<dynamic> reviews, int filterIndex)
        {
            if (filterIndex == 0)
            {
                return reviews;
            }
            return reviews.Where(r => r?.stars != null && (int)r.stars == filterIndex).ToList();
        }

        private void NotFound()
        {
            if (ReviewsPanel.Children.Count == 0)
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


        private void backB_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ReviewsPanel != null)
            {
                if (!string.IsNullOrEmpty(searchTB.Text))
                    search.Visibility = Visibility.Hidden;
                else
                    search.Visibility = Visibility.Visible;
                if (string.IsNullOrEmpty(searchTB.Text) && _allReviews.Count == 0)
                {
                    LoadReviews();
                }
                else
                {
                    DisplayReviews(_allReviews);
                }
            }
        }
        private void sordCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReviewsPanel != null && _allReviews.Count > 0)
            {
                DisplayReviews(_allReviews);
                NotFound();
            }
        }

        private void filterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReviewsPanel != null && _allReviews.Count > 0)
            {
                DisplayReviews(_allReviews);
                NotFound();
            }
        }
    }
}
