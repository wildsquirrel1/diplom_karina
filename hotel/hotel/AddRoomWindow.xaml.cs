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
    /// Логика взаимодействия для AddRoomWindow.xaml
    /// </summary>
    public partial class AddRoomWindow : Window
    {
        Employee employee;
        private EditRoomWindow editRoomWindow;
        private List<Floor> _floors = new();
        private List<Category> _categories = new();
        private List<StatusRoom> _statuses = new();
        public AddRoomWindow(Employee currentEmployee)
        {
            InitializeComponent();
            employee = currentEmployee;
            emplName.Text += " " + employee.Name + " " + employee.Lastname;
            emplRole.Text += " " + employee.IdroleNavigation.Name;
            editRoomWindow = new EditRoomWindow();
            LoadData();
        }
        private async void LoadData()
        {
            _floors = await Api.GetFloors();
            _categories = await Api.GetCategories();
            _statuses = await Api.GetStatuses();

            floorCB.ItemsSource = _floors.Select(f => f.Name).ToList();
            categoryCB.ItemsSource = _categories.Select(c => c.Name).ToList();
            statusCB.ItemsSource = _statuses.Select(s => s.Name).ToList();
            statusCB.SelectedIndex = _statuses.FindIndex(s => s.IdstatusRoom == 2);
            categoryCB.SelectionChanged += categoryCB_SelectionChanged;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void nameTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !editRoomWindow.IsDigit(e.Text);
        }
        
        private async void addB_Click(object sender, RoutedEventArgs e)
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
            int categoryId = _categories[categoryCB.SelectedIndex].Idcategory;
            if (!editRoomWindow.IsRoomNumberValid(nameTB.Text, floorId))
            {
                MessageBox.Show("Первая цифра номера комнаты должна совпадать с номером этажа", "Уведомление");
                return;
            }
            if(editRoomWindow.Restrictions(nameTB.Text, floorCB.SelectedIndex, categoryCB.SelectedIndex, statusCB.SelectedIndex))
            {
                await addRoom(nameTB.Text, floorId, categoryId);
                MessageBox.Show("Комната успешно добавлена", "Уведомление");
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Проверьте данные еще раз!", "Уведомление");
                return;
            }
        }

        private async Task addRoom(string name, int floorid, int categiryid )
        {
            var newRoom = new Room
            {
                Name = name,
                Floorid = floorid,
                IdCategory = categiryid,
                StatusId = 2,
                Hotelid = employee.IdhotelNavigation.Idhotel
            };

            await Api.AddRoom(newRoom);
        }

        private void categoryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoryCB.SelectedIndex != -1 && _categories != null)
            {
                var selectedCategory = _categories[categoryCB.SelectedIndex];

                priceTB.Text = selectedCategory.Cost.ToString();
            }
            else
            {
                priceTB.Text = "0";
            }
        }
    }
}
