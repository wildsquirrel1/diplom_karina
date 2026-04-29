using hotel.Data;
using hotel.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hotel
{
    /// <summary>
    /// Логика взаимодействия для HotelControl.xaml
    /// </summary>
    public partial class HotelControl : UserControl
    {
        Hotel hotel1;
        private Employee _currEmpl;
        public HotelControl(Employee employee)
        {
            InitializeComponent();
            this.DataContext = this;
            _currEmpl = employee;
        }

        public async Task DataHotel(Hotel hotel)
        {
            hotel1 = hotel;
            nameH.Text = "Название: " + hotel.Name;
            addressH.Text = "Адрес: " + hotel.Address;
            cityH.Text = "Город: " + hotel.City;
            phoneH.Text = "Номер телефона: " + hotel.PhoneNumber;
            emailH.Text = "Электронная почта: " + hotel.Email;

            var ratingData = await Api.GetHotelRatingAsync(hotel.Idhotel);
            if (ratingData.HasValue)
            {
                var (rating, count) = ratingData.Value;
                starsH.Text = rating.HasValue
                    ? $"⭐ Рейтинг: {rating}/5 ({FormatReviewsCount(count)})"
                    : "⭐ Рейтинг: Нет отзывов";
            }
            else
            {
                starsH.Text = "⭐ Рейтинг: Ошибка загрузки";
            }

            if (hotel.Photo != null && hotel.Photo.Length > 0)
            {
                try
                {
                    var ms = new MemoryStream(hotel.Photo);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    photo.Source = bitmap;
                }
                catch
                {
                    photo.Source = GetDefaultImage();
                }
            }
            else
            {
                photo.Source = GetDefaultImage();
            }
        }

        private ImageSource GetDefaultImage()
        {
            try
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/a-none-logo.jpg"));
            }
            catch
            {
                return null;
            }
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            var edit = new EditHotelWindow(hotel1, _currEmpl);
            bool? result = edit.ShowDialog();

            if (result == true && HotelsWindow.Instance != null)
            {
                HotelsWindow.Instance.loadHotels();
            }
        }

        private string FormatReviewsCount(int count)
        {
            if (count == 1) return $"{count} отзыв";
            if (count >= 2 && count <= 4) return $"{count} отзыва";
            return $"{count} отзывов";
        }

        private void reviewsBtn_Click(object sender, RoutedEventArgs e)
        {
            var review = new ReviewWindow(hotel1.Idhotel);
            bool? result = review.ShowDialog();

            if(result == true && HotelsWindow.Instance != null)
            {
                HotelsWindow.Instance.loadHotels();
            }
        }
    }
}
