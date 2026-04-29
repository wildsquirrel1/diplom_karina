using hotel.Data;
using hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для EditEmployeeWindow.xaml
    /// </summary>
    public partial class EditEmployeeWindow : Window
    {
        private DateTime bdt => birth.SelectedDate ?? DateTime.MinValue;
        private DateOnly bdat => DateOnly.FromDateTime(bdt);

        private Employee _employee;
        private Employee _currentUser;
        public EditEmployeeWindow(Employee employee, Employee currentUser)
        {
            InitializeComponent();

            _employee = employee;
            _currentUser = currentUser;

            emplName.Text += " " + _currentUser.Name + " " +  _currentUser.Lastname;
            emplRole.Text += " " + _currentUser.IdroleNavigation.Name;

            LoadHotelsAsync();

            lastnameT.Text = employee.Lastname;
            nameT.Text = employee.Name;
            patronymicT.Text = employee.Patronymic;
            phoneT.Text = employee.PhoneNumber;
            emailT.Text = employee.Email;
            passwordPB.Text = employee.Password;
            birth.SelectedDate = new DateTime(employee.Birth.Year, employee.Birth.Month, employee.Birth.Day);
        }

        public EditEmployeeWindow() { }

        private async void LoadHotelsAsync()
        {
            var hotels = await Api.GetHotels();
            foreach (var hotel in hotels)
            {
                hotelCB.Items.Add(hotel.Name);
            }
            hotelCB.SelectedIndex = (_employee.Idhotel ?? 0) - 1;;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            /*EmployeeHHMWindow employeeHHMWindow = new EmployeeHHMWindow(_currentUser);
            employeeHHMWindow.Show();*/
        }

        private void lastnameT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Preview(e);
        }

        private void nameT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Preview(e);
        }

        private void patronymicT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Preview(e);
        }

        private void phoneT_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDownNumber(e);
        }

        private void birthDP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private async void editB_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(nameT.Text) ||
                string.IsNullOrEmpty(lastnameT.Text) ||
                string.IsNullOrEmpty(phoneT.Text) ||
                string.IsNullOrEmpty(emailT.Text) ||
                string.IsNullOrEmpty(passwordPB.Text) ||
                birth.SelectedDate == null ||
                hotelCB.SelectedIndex == -1)
            {
                MessageBox.Show("Заполните все обязательные поля", "Уведомление");
                
                return;
            }

            if (Restrictions(lastnameT.Text, nameT.Text, patronymicT.Text, phoneT.Text, emailT.Text, passwordPB.Text, birth, hotelCB.SelectedIndex))
            {
                await EditEmp(lastnameT.Text, nameT.Text, patronymicT.Text, phoneT.Text, emailT.Text, passwordPB.Text, hotelCB.SelectedIndex + 1);
                MessageBox.Show("Сотрудник успешно отредактирован!", "Уведомление");
                this.DialogResult = true;
                this.Close();
            }
        }

        public bool Restrictions(string lastname, string name, string patronymic, string phonenumber, string email, string password, DatePicker bd, int roleid)
        {
            if (!IsValidFI(lastname))
            {
                MessageBox.Show("Фамилия должна содержать только кириллицу и дефис", "Уведомление");
                return false;
            }
            if (!IsValidFI(name))
            {
                MessageBox.Show("Имя должно содержать только кириллицу и дефис", "Уведомление");
                return false;
            }
            if (!IsValidPatr(patronymic))
            {
                MessageBox.Show("Отчество должно содержать только кириллицу и дефис", "Уведомление");
                return false;
            }
            if (!IsValidNumber(phonenumber))
            {
                MessageBox.Show("Неверный формат телефона. Пример: 89123456789", "Уведомление");
                return false;
            }
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Неверный формат электронной почты", "Уведомление");
                return false;
            }
            if (!IsValidBirthDate(bd))
            {
                MessageBox.Show("Сотрудник должен быть старше 18 лет", "Уведомление");
                return false;
            }
            return true;
        }

        public async Task EditEmp(string lastname, string name, string patronymic, string phoneNum, string email, string password, int hotelId)
        {
            _employee.Lastname = lastname;
            _employee.Name = name;
            _employee.Patronymic = patronymic;
            _employee.PhoneNumber = phoneNum;
            _employee.Email = email;
            _employee.Password = password;
            _employee.Idhotel = hotelId;
            _employee.Status = 0;
            _employee.Birth = DateOnly.FromDateTime(birth.SelectedDate.Value);

            await Api.UpdateEmployee(_employee.Idemployee, _employee);
        }

        public bool IsValidNumber(string number)
        {
            var regex_for_phone = new Regex(@"^\\bc\+7|8[0-9]{10}$");

            if (string.IsNullOrEmpty(number))
                return false;
            else if (!regex_for_phone.IsMatch(number))
                return false;
            else return true;
        }

        public bool IsValidFI(string fi)
        {
            Regex alp = new Regex("[A-zA-Z-]+$");

            if (string.IsNullOrEmpty(fi) || fi.Length > 55)
            {
                return false;
            }
            if (alp.IsMatch(fi))
                return false;
            return true;
        }

        public bool IsValidPatr(string patr)
        {
            Regex alp = new Regex("[A-zA-Z-]+$");

            if (string.IsNullOrEmpty(patr))
                return true;
            if (alp.IsMatch(patr))
                return false;
            if (patr.Length > 45)
                return false;
            return true;
        }

        public bool IsValidEmail(string email)
        {
            Regex mail = new Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.ru|com$");
            if (string.IsNullOrEmpty(email))
                return false;
            if (email.Length > 100) return false;
            if (!mail.IsMatch(email)) return false;
            return true;
        }

        public bool IsValidPassword(string password)
        {
            return password.Length <= 25;
        }

        public bool IsValidBirthDate(DatePicker dp)
        {
            if (!dp.SelectedDate.HasValue) return false;

            DateTime selectedDate = dp.SelectedDate.Value;
            DateTime today = DateTime.Today;
            return selectedDate >= today.AddYears(-100) && selectedDate <= today.AddYears(-18);
        }

        public void Preview(TextCompositionEventArgs e)
        {
            if (!IsRussianLetter(e.Text))
            {
                e.Handled = true;
            }
            if (e.Text == "-")
            {
                e.Handled = false;
            }
        }

        public void KeyDownNumber(KeyEventArgs e)
        {
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                e.Handled = true;
            }
        }

        public bool IsTextAllowed(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c) && c != '.')
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsRussianLetter(string text)
        {
            foreach (char c in text)
            {
                if ((c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'))
                {
                    return true;
                }
            }
            return false;
        }

        public string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            char firstChar = char.ToUpper(input[0]);
            string restOfTheString = input.Substring(1).ToLower();

            return firstChar + restOfTheString;
        }
    }
}
