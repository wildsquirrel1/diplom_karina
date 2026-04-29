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
        Employee employee;
        public ReviewWindow(int hotelId)
        {
            InitializeComponent();
            _hotelId = hotelId;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadReviews();
        }

        private async Task LoadReviews()
        {
            ReviewsPanel.Children.Clear();

            var reviews = await Api.GetHotelReviewsAsync(_hotelId);

            if (reviews == null || reviews.Count == 0)
            {
                ReviewsPanel.Children.Add(new TextBlock
                {
                    Text = "Отзывов пока нет",
                    FontSize = 16,
                    Margin = new Thickness(10)
                });
                return;
            }

            foreach (var review in reviews)
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

        private void backB_Click(object sender, RoutedEventArgs e)
        {

        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void sordCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void filterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
