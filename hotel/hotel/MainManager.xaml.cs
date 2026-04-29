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
    /// Логика взаимодействия для MainManager.xaml
    /// </summary>
    public partial class MainManager : Window
    {
        private Employee _Employee;
        public MainManager(Employee employee)
        {
            InitializeComponent();
            _Employee = employee;
            emplName.Text += " " + employee.Name + " " + employee.Lastname;
            emplRole.Text += " " + employee.IdroleNavigation.Name;
            emplHotel.Text += " " + employee.IdhotelNavigation.Name;
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Autorisation autorisation = new Autorisation();
            autorisation.Show();
        }

        private void employees_Click(object sender, RoutedEventArgs e)
        {
            EmployeeHHMWindow employeeHHMWindow = new EmployeeHHMWindow(_Employee);
            employeeHHMWindow.Show();
            this.Hide();
        }

        private void rooms_Click(object sender, RoutedEventArgs e)
        {
            RoomsWindow roomsWindow = new RoomsWindow(_Employee);
            roomsWindow.Show();
            this.Hide();
        }

        private void services_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
