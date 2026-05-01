using Castle.Components.DictionaryAdapter.Xml;
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
    /// Логика взаимодействия для RoomsWindow.xaml
    /// </summary>
    public partial class RoomsWindow : Window
    {
        private List<Room> _allRooms = new();
        private Employee _currentUser;

        public RoomsWindow(Employee currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            emplName.Text += $" {currentUser.Name} {currentUser.Lastname}";
            emplRole.Text += " " + currentUser.IdroleNavigation.Name;
            LoadRoomsAsync();
        }

        public async Task LoadRoomsAsync()
        {
            roomList.Children.Clear();
            try
            {
                _allRooms = await Api.GetRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки номеров: {ex.Message}", "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var filtered = ApplySearch(_allRooms, searchTB.Text?.ToLower());
            filtered = ApplySort(filtered, sortBox.SelectedIndex);
            filtered = ApplyFilter(filtered, filterBox.SelectedIndex);

            foreach (var room in filtered)
            {
                var control = new RoomControl(_currentUser);
                control.SetRoom(room);  // ← Здесь фото загрузятся асинхронно
                roomList.Children.Add(control);
            }

            if (roomList.Children.Count == 0)
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

        private List<Room> ApplySearch(List<Room> rooms, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return rooms;
            return rooms.Where(r => r.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private List<Room> ApplySort(List<Room> rooms, int index)
        {
            return index switch
            {
                0 => rooms.OrderByDescending(r => r.Idroom).ToList(),
                1 => rooms.OrderBy(r => r.Name).ToList(),
                2 => rooms.OrderByDescending(r => r.Name).ToList(),
                _ => rooms.OrderByDescending(r => r.Idroom).ToList()
            };
        }

        private List<Room> ApplyFilter(List<Room> rooms, int index)
        {
            return index switch
            {
                0 => rooms,
                1 => rooms.Where(r => r.StatusId == 2).ToList(), 
                2 => rooms.Where(r => r.StatusId == 1).ToList(), 
                3 => rooms.Where(r => r.StatusId == 3).ToList(), 
                _ => rooms
            };
        }

        private async void sortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(roomList !=  null)
            {
                await LoadRoomsAsync();
                NotFound();
            }
        }

        private async void filterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (roomList != null)
            {
                await LoadRoomsAsync();
                NotFound();
            }
        }

        private async void addRoom_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddRoomWindow(_currentUser);
            if (addWindow.ShowDialog() == true)
            {
                await LoadRoomsAsync();
            }
        }

        private async void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchTB.Text))
            {
                search.Visibility = Visibility.Hidden;
            }
            else
            {
                search.Visibility = Visibility.Hidden;
            }
            if (roomList != null)
            {
                await LoadRoomsAsync();
                NotFound();
            }
        }

        public void NotFound()
        {
            if (roomList.Children.Count == 0 || roomList == null)
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

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainManager mainManager = new MainManager(_currentUser);
            mainManager.Show();
        }
    }
}
