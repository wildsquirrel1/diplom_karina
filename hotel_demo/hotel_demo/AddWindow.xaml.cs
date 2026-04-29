using hotel_demo.Model;
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

namespace hotel_demo
{
    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        EditWindow editWindow = new EditWindow();

        private DateTime bdt => birth.SelectedDate ?? DateTime.MinValue;
        private DateOnly bdate => DateOnly.FromDateTime(bdt);
        private Employeer newEmployeer = new Employeer();
        public AddWindow()
        {
            InitializeComponent();
            //this.editWindow = editWindow;
            DataContext = this.newEmployeer;
        }

        private void AddEmployee(string lastname, string name, string patronymic, string email, string password, DateOnly bday)
        {
            newEmployeer.Lastname = lastname;
            newEmployeer.Name = name;
            newEmployeer.Patronymic = patronymic;
            newEmployeer.Email = email;
            newEmployeer.Password = password;
            newEmployeer.Birth = bday;
            newEmployeer.Isblocked = false;
            newEmployeer.Roleid = 2;

            App.contex.Employeers.Add(newEmployeer);
            App.contex.SaveChanges();
        }

        private void birth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !editWindow.IsTextAllowed(e.Text);
        }

        private void lastnameTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            editWindow.Preview(e);
        }

        private void nameTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            editWindow.Preview(e);
        }

        private void patronymicTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            editWindow.Preview(e);
        }

        private void addB_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(lastnameTB.Text) || string.IsNullOrEmpty(nameTB.Text) ||
                string.IsNullOrEmpty(patronymicTB.Text) || string.IsNullOrEmpty(emailTB.Text) ||
                string.IsNullOrEmpty(passwordTB.Text) || string.IsNullOrEmpty(birth.Text))
            {
                MessageBox.Show("Заполните пустые поля!", "Ошибка");
                return;
            }
            if(editWindow.Restricions(lastnameTB.Text, nameTB.Text, patronymicTB.Text, emailTB.Text, passwordTB.Text, birth))
            {
                AddEmployee(lastnameTB.Text, nameTB.Text, patronymicTB.Text, emailTB.Text, passwordTB.Text, bdate);
                MessageBox.Show("Сотрудник успешно добавлен", "Успешно");
                
                this.Hide();
                ResetParams();

                AdminWindow adminWindow = new AdminWindow();
                adminWindow.Show();
            }
        }

        private void ResetParams()
        {
            lastnameTB.Text = string.Empty;
            nameTB.Text = string.Empty;
            patronymicTB.Text = string.Empty;
            emailTB.Text = string.Empty;
            passwordTB.Text = string.Empty;
            birth.Text = string.Empty;
        }
    }
}
