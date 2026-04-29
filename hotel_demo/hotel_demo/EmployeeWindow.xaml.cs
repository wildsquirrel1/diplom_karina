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
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        private Employeer employeer;
        public EmployeeWindow(Employeer employee)
        {
            InitializeComponent();
            this.employeer = employee;
            currPass.Text = employee.Password;
        }

        private void changePass_Click(object sender, RoutedEventArgs e)
        {
            if (newPass.Text == "" && cofirmNewPass.Text == "")
            {
                MessageBox.Show("Все поля должны быть заполнены", "Ошибка");
                return;
            }
            else if ( newPass.Text != cofirmNewPass.Text)
            {
                MessageBox.Show("Пароли должны совпадать!", "Ошибка ");
                return;
            }
            if(currPass.Text == newPass.Text ||  currPass.Text == cofirmNewPass.Text)
            {
                MessageBox.Show("Новый пароль не должен совпадать со старым");
                return;
            }
            else
            {
                employeer.Password = newPass.Text;
                App.contex.SaveChanges();
                MessageBox.Show("Пароль обновлен", "Успешно");
                this.Hide();
            }
        }
    }
}
