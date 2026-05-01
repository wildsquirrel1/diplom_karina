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
    /// Логика взаимодействия для EditRoomWindow.xaml
    /// </summary>
    public partial class EditRoomWindow : Window
    {
        private Room _room;
        private Employee _currentUser;
        private List<Floor> _floors = new();
        private List<Category> _categories = new();
        private List<StatusRoom> _statuses = new();
        public EditRoomWindow(Room room, Employee currentUser)
        {
            InitializeComponent();
            _room = room;
            _currentUser = currentUser;
            emplName.Text += $"{currentUser.Name} {currentUser.Lastname}";
            emplRole.Text += " " + currentUser.IdroleNavigation.Name;
            LoadDataAsync();
        }

        public EditRoomWindow() {}

        private async void LoadDataAsync()
        {
            _floors = await Api.GetFloors();
            _categories = await Api.GetCategories();
            _statuses = await Api.GetStatuses();

            floorCB.ItemsSource = _floors.Select(f => f.Name).ToList();
            categoryCB.ItemsSource = _categories.Select(c => c.Name).ToList();
            statusCB.ItemsSource = _statuses.Select(s => s.Name).ToList();

            floorCB.SelectedIndex = _floors.FindIndex(f => f.Idfloor == _room.Floorid);
            categoryCB.SelectedIndex = _categories.FindIndex(c => c.Idcategory == _room.IdCategory);
            statusCB.SelectedIndex = _statuses.FindIndex(s => s.IdstatusRoom == _room.StatusId);
            nameTB.Text = _room.Name;
            priceTB.Text = _room.IdCategoryNavigation?.Cost.ToString() ?? "0";

            CheckAndLockIfBusy();
        }

        private void CheckAndLockIfBusy()
        {
            if (_room.StatusId == 1)
            {
                nameTB.IsEnabled = false;
                floorCB.IsEnabled = false;
                categoryCB.IsEnabled = false;
                statusCB.IsEnabled = false;
                editB.IsEnabled = false;
                MessageBox.Show(
                    "Невозможно редактировать данные номера, так как он имеет статус «Занят».\nСначала освободите номер.",
                    "Уведомление",
                    MessageBoxButton.OK
                );
            }
        }

        private void nameTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDigit(e.Text);
        }

        public void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void editB_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTB.Text) ||
                floorCB.SelectedIndex == -1 ||
                categoryCB.SelectedIndex == -1 ||
                statusCB.SelectedIndex == -1)
            {
                MessageBox.Show("Заполните все обязательные поля", "Уведомление");
                return;
            }

            int floorId = _floors[floorCB.SelectedIndex].Idfloor;
            if (!IsRoomNumberValid(nameTB.Text, floorId))
            {
                MessageBox.Show("Первая цифра номера комнаты должна совпадать с номером этажа", "Уведомление");
                return;
            }

            if (Restrictions(nameTB.Text, floorCB.SelectedIndex, categoryCB.SelectedIndex, statusCB.SelectedIndex))
            {
                await EditRoom(
                    nameTB.Text,
                    floorId,
                    _categories[categoryCB.SelectedIndex].Idcategory,
                    _statuses[statusCB.SelectedIndex].IdstatusRoom
                );
                MessageBox.Show("Комната успешно отредактирована!", "Уведомление");
                DialogResult = true;
                this.Close();
                /*nameTB.Text = "";
                floorCB.SelectedIndex = -1;
                categoryCB.SelectedIndex = -1;
                priceTB.Text = "";
                statusCB.SelectedIndex = -1;*/
            }
            else
            {
                MessageBox.Show("Проверьте данные еще раз!", "Уведомление");
                return;
            }
        }

        public bool IsDigit(string text)
        {
            return text.All(char.IsDigit);
        }

        public bool IsRoomNumberValid(string roomNumber, int floorId)
        {
            if (string.IsNullOrWhiteSpace(roomNumber)) return false;
            if (!roomNumber.All(char.IsDigit)) return false;
            int firstDigit = int.Parse(roomNumber[0].ToString());
            return firstDigit == floorId;
        }

        public async Task EditRoom(string name, int floorId, int categoryId, int statusId)
        {
            _room.Name = name;
            _room.Floorid = floorId;
            _room.IdCategory = categoryId;
            _room.StatusId = statusId;
            _room.Hotelid = _currentUser.IdhotelNavigation.Idhotel;
            await Api.UpdateRoom(_room.Idroom, _room);
        }

        public bool Restrictions(string name, int floorId, int categoryId, int statusId)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 10)
            {
                MessageBox.Show("Название должно быть от 1 до 10 символов", "Уведомление");
                return false;
            }
            if (floorId <= -1)
            {
                MessageBox.Show("Выберите этаж", "Уведомление");
                return false;
            }
            if (categoryId <= -1)
            {
                MessageBox.Show("Выберите категорию", "Уведомление");
                return false;
            }
            if (statusId <= -1)
            {
                MessageBox.Show("Выберите статус", "Уведомление");
                return false;
            }
            return true;
        }
    }
}
