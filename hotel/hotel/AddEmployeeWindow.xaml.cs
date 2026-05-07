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
    /// Логика взаимодействия для AddEmployeeWindow.xaml
    /// </summary>
    public partial class AddEmployeeWindow : Window
    {
        private Employee _currentUser;
        private List<Hotel> _hotels = new();
        private EditEmployeeWindow _editWindowRef;
        private DateTime bdt => birth.SelectedDate ?? DateTime.MinValue;
        private DateOnly bdat => DateOnly.FromDateTime(bdt);
        public AddEmployeeWindow(Employee currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _editWindowRef = new EditEmployeeWindow();
            emplName.Text += $" {currentUser.Name} {currentUser.Lastname}";
            emplRole.Text += " " + currentUser.IdroleNavigation.Name;
            LoadHotelsAsync();
        }
        private async void LoadHotelsAsync()
        {
            _hotels = await Api.GetHotels();
            hotelCB.ItemsSource = _hotels.Select(h => h.Name).ToList();

            // Если главный менеджер (роль 2) — выбираем его отель и блокируем
            if (_currentUser.Idrole == 2 && _currentUser.IdhotelNavigation != null)
            {
                int index = _hotels.FindIndex(h => h.Idhotel == _currentUser.IdhotelNavigation.Idhotel);
                if (index >= 0)
                {
                    hotelCB.SelectedIndex = index;
                    hotelCB.IsEnabled = false; // Блокируем поле
                }
            }
            else if (_hotels.Count > 0)
            {
                hotelCB.SelectedIndex = 0;
            }
        }

        private void lastnameT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            _editWindowRef.Preview(e);

        }

        private void nameT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            _editWindowRef.Preview(e);
        }

        private void patronymicT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            _editWindowRef.Preview(e);
        }

        private void phoneT_KeyDown(object sender, KeyEventArgs e)
        {
            _editWindowRef.KeyDownNumber(e);
        }

        private void birth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_editWindowRef.IsTextAllowed(e.Text);
        }

        private async void addB_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(lastnameT.Text) ||
                string.IsNullOrEmpty(nameT.Text) ||
                string.IsNullOrEmpty(phoneT.Text) ||
                string.IsNullOrEmpty(emailT.Text) ||
                string.IsNullOrEmpty(passwordPB.Password) ||
                birth.SelectedDate == null ||
                hotelCB.SelectedIndex == -1)
            {
                MessageBox.Show("Заполните все обязательные поля", "Уведомление");
                
                return;
            }

            if (_editWindowRef.Restrictions(
                    lastnameT.Text, nameT.Text, patronymicT.Text,
                    phoneT.Text, emailT.Text, passwordPB.Password,
                    birth, hotelCB.SelectedIndex))
            {
                await AddEmp(
                    lastnameT.Text, nameT.Text, patronymicT.Text,
                    phoneT.Text, emailT.Text, passwordPB.Password,
                    hotelCB.SelectedIndex + 1);

                MessageBox.Show("Сотрудник успешно добавлен!", "Уведомление");
                DialogResult = true;
                Close();
            }
        }

        public async Task AddEmp(string lastname, string name, string patronymic,
                           string phoneNum, string email, string password, int hotelId)
        {
            var newEmployee = new Employee
            {
                Lastname = _editWindowRef.CapitalizeFirstLetter(lastname),
                Name = _editWindowRef.CapitalizeFirstLetter(name),
                Patronymic = _editWindowRef.CapitalizeFirstLetter(patronymic),
                PhoneNumber = phoneNum,
                Email = email,
                Password = password,
                Birth = DateOnly.FromDateTime(birth.SelectedDate.Value),
                Idhotel = hotelId,
                Idrole = _currentUser.Idrole == 1 ? 2 : 3,
                Status = 1
            };

            await Api.AddEmployee(newEmployee);
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

        }
    }
}
