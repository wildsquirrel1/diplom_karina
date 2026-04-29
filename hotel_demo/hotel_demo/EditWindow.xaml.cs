using hotel_demo.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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

namespace hotel_demo
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {

        private DateTime bdt => birth.SelectedDate ?? DateTime.MinValue;
        private DateOnly bdate => DateOnly.FromDateTime(bdt);

        private Employeer selectedEmployee;
        public EditWindow(Employeer employeer)
        {
            InitializeComponent();
            this.selectedEmployee = employeer;
            nameTB.Text = selectedEmployee.Name;
            lastnameTB.Text = selectedEmployee.Lastname;
            patronymicTB.Text = selectedEmployee.Patronymic;
            emailTB.Text = selectedEmployee.Email;
            passwordTB.Text = selectedEmployee.Password;

            birth.SelectedDate = new DateTime(selectedEmployee.Birth.Year,
                selectedEmployee.Birth.Month, selectedEmployee.Birth.Day);

        }

        public EditWindow() { }

        private void editB_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(nameTB.Text) ||string.IsNullOrEmpty(lastnameTB.Text)
                || string.IsNullOrEmpty(patronymicTB.Text) || string.IsNullOrEmpty(emailTB.Text)
                || string.IsNullOrEmpty(passwordTB.Text) || idstatus.SelectedIndex == -1 || string.IsNullOrEmpty(birth.Text))
            {
                MessageBox.Show("Заполните пустые поля", "Ошибка");
                return;
            }
            if(Restricions(lastnameTB.Text, nameTB.Text, patronymicTB.Text, emailTB.Text, passwordTB.Text, birth))
            {
                EditEmp(lastnameTB.Text, nameTB.Text, patronymicTB.Text, emailTB.Text, passwordTB.Text);
                MessageBox.Show("Сотрудник успешно отредактирован", "Успешно");
                ResetParams();
                this.Hide();
                AdminWindow adminWindow = new AdminWindow();
                adminWindow.Show();
            }
        }

        public void ResetParams()
        {
            lastnameTB.Text = string.Empty;
            nameTB.Text = string.Empty;
            patronymicTB.Text = string.Empty;
            emailTB.Text = string.Empty;
            passwordTB.Text = string.Empty;
            birth.Text = string.Empty;
            idstatus.SelectedIndex = -1;
        }
        private void lastnameTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Preview(e);
        }

        private void nameTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Preview(e);
        }

        private void patronymicTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Preview(e);
        }

        public void EditEmp(string lastname, string name, string patronymic, string email, string password)
        {
            var user = App.contex.Employeers.Find(selectedEmployee.Idemployeer);

            user.Lastname = lastname;
            user.Name = name;
            user.Patronymic = patronymic;
            user.Email = email;
            user.Password = password;
            user.Birth = bdate;
            
            App.contex.SaveChanges();
        }

        public bool Restricions (string lastname, string name, string patronymic, string email, string password, DatePicker bd)
        {
            lastname = CapitalizeFirstLetter(lastname);
            name = CapitalizeFirstLetter(name);
            patronymic = CapitalizeFirstLetter(patronymic);

            if (!IsValidFI(lastname))
            {
                MessageBox.Show("Фамилия не может быть пустой, содержать цифры, символы и английский алфавит", "Ошибка");
                return false;
            }
            if(!IsValidFI(name))
            {
                MessageBox.Show("Имя не может быть пустым, содержать цифры, символы и английский алфавит", "Ошибка");
                return false;
            }
            if (!IsValidPatr(patronymic))
            {
                MessageBox.Show("Отчество не может содержать цифры, символы или английский алфавит", "Ошибка");
                return false;
            }
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Не верный формат электронной почты", "Ошибка");
                return false;
            }
            if (!IsUniqueEmail(email))
            {
                MessageBox.Show("Эта электронная почта уже кем-то используется", "Ошибка");
                return false;
            }
            if (!IsValidPassword(password))
            {
                MessageBox.Show("Слишком длинный пароль", "Ошибка")
                ; return false;
            }
            if (!IsValidBirth(bd))
            {
                MessageBox.Show("Сотрудник не может быть старше 100 лет и младше 18 лет", "Ошибка");
                return false ;
            }

            return true;
        }
        public bool IsValidFI(string fi)
        {
            Regex alp = new Regex("[A-zA-Z-]+$");

            if(string.IsNullOrEmpty(fi) || fi.Length > 45)
                return false;
            if (alp.IsMatch(fi))
                return false;
            return true;
        }

        public bool IsValidEmail(string email)
        {
            Regex mail = new Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.ru|.com");

            if (string.IsNullOrEmpty(email))
                return false;
            if (email.Length > 150) return false;
            if(!mail.IsMatch(email)) return false;
            return true;
        }

        public bool IsUniqueEmail(string email)
        {
            if (selectedEmployee != null)
            {
                return !App.contex.Employeers.Any(u => u.Email == email && u.Email != selectedEmployee.Email);
            }

            return !App.contex.Employeers.Any(u => u.Email == email);
        }

        public bool IsValidPatr(string part)    
        {
            Regex alp = new Regex("[A-zA-Z-]+$");

            if (string.IsNullOrEmpty(part) || part.Length > 45)
                return true;
            if (alp.IsMatch(part))
                return false;
            return true;
        }

        public bool IsValidPassword(string password)
        {
            return password.Length <= 25;
        }

        public bool IsValidBirth(DatePicker dp)
        {
            if (!dp.SelectedDate.HasValue)
            {
                return false;
            }
            DateTime selectedDate = dp.SelectedDate.Value;
            DateTime today = DateTime.Today;
            return selectedDate >= today.AddYears(-100) && selectedDate <= today.AddYears(-18);
        }
        
        public static string CapitalizeFirstLetter(string input)
        {
            if(string.IsNullOrEmpty(input))
            {
                return input;
            }

            char firstChar = char.ToUpper(input[0]);

            string restOfTheString = input.Substring(1);

            return firstChar + restOfTheString;
        }

        public bool IsTextAllowed(string text)
        {
            foreach(char c in text)
            {
                if(!char.IsDigit(c) && c != '.')
                    return false;
            }

            return true;
        }

        public void Preview(TextCompositionEventArgs e)
        {
            if(!IsRussianLetter(e.Text))
            {
                e.Handled = true;
            }
            if(e.Text == "-")
            {
                e.Handled = false;
            }
        }

        public bool IsRussianLetter(string text)
        {
            foreach(char c in text)
            {
                if((c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'))
                {
                    return true;
                }
            }
            return false;
        }

        private void birth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}
