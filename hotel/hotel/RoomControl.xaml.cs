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
    /// Логика взаимодействия для RoomControl.xaml
    /// </summary>
    public partial class RoomControl : UserControl
    {
        private Room _room;
        private int _currentPhotoIndex = 0;
        private List<byte[]> _photos = new();
        private Employee _currentUser;

        public RoomControl(Employee employee)
        {
            InitializeComponent();
            _currentUser = employee;
        }

        public async void SetRoom(Room room)
        {
            _room = room;
            DataContext = _room;

            RefreshDisplay();

            photoDisplay.Source = GetDefaultImage();

            await LoadPhotosAsync();
        }

        private async Task LoadPhotosAsync()
        {
            _photos = await Api.GetRoomPhotosAsync(_room.Idroom);

            if (_photos.Count == 0)
            {
                _photos.Add(new byte[0]);
            }

            _currentPhotoIndex = 0;

            prevBtn.Visibility = _photos.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
            nextBtn.Visibility = _photos.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

            UpdatePhotoDisplay();
        }

        private void UpdatePhotoDisplay()
        {
            if (_photos.Count == 0) return;

            var imageData = _photos[_currentPhotoIndex];
            if (imageData.Length == 0)
            {
                photoDisplay.Source = GetDefaultImage();
            }
            else
            {
                try
                {
                    using var ms = new System.IO.MemoryStream(imageData);
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    photoDisplay.Source = bitmap;
                }
                catch
                {
                    photoDisplay.Source = GetDefaultImage();
                }
            }
        }

        private ImageSource GetDefaultImage()
        {
            try
            {
                return new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/images/a-none-logo.jpg"));
            }
            catch
            {
                return null;
            }
        }

        private void prevBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_photos.Count <= 1) return;
            _currentPhotoIndex = (_currentPhotoIndex - 1 + _photos.Count) % _photos.Count;
            UpdatePhotoDisplay();
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_photos.Count <= 1) return;
            _currentPhotoIndex = (_currentPhotoIndex + 1) % _photos.Count;
            UpdatePhotoDisplay();
        }

        public void RefreshDisplay()
        {
            if (_room == null) return;

            roomName.Text = $"Комната номер: {_room.Name}";
            floorName.Text = $"Этаж: {_room.Floorid}";
            categoryName.Text = $"Категория: {_room.IdCategoryNavigation?.Name ?? "—"}";
            priceCategory.Text = $"{_room.IdCategoryNavigation?.Cost ?? 0} ₽";
            statusName.Text = _room.Status?.Name ?? "—";
        }

        private async void editB_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditRoomWindow(_room, _currentUser);
            if (editWindow.ShowDialog() == true)
            {
                RefreshDisplay();

                var parentWindow = Window.GetWindow(this) as RoomsWindow;
                if (parentWindow != null)
                    await parentWindow.LoadRoomsAsync();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            prevBtn.Click += prevBtn_Click;
            nextBtn.Click += nextBtn_Click;
        }
    }
}
