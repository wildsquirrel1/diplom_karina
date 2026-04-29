using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using hotel.Data;
using hotel.Models;

namespace hotel
{
    /// <summary>
    /// Логика взаимодействия для HeadHotelManagerWindow.xaml
    /// </summary>
    public partial class HeadHotelManagerWindow : Window
    {
        Employee Employee;
        public HeadHotelManagerWindow(Employee employee)
        {
            InitializeComponent();
            this.Employee = employee;
            GetEmpl(employee, emplName, emplRole);
        }

        public HeadHotelManagerWindow() { }

        public async void GetEmpl(Employee employee, TextBlock empName, TextBlock roleName)
        {
            var res = await Api.Auth(email: employee.Email.ToString(), password: employee.Password.ToString());
            empName.Text += res.Lastname.ToString() + " " + res.Name.ToString();
            roleName.Text += res.IdroleNavigation.Name.ToString();
        }

        private void hotels_Click(object sender, RoutedEventArgs e)
        {
            HotelsWindow hotelsWindow = new HotelsWindow(this.Employee);
            hotelsWindow.Show();
            this.Hide();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Autorisation autorisation = new Autorisation();
            autorisation.Show();
        }

        private void reports_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            BookingReportWindow bookingReportWindow = new BookingReportWindow(Employee);
            bookingReportWindow.Show();
        }

        private void employees_Click(object sender, RoutedEventArgs e)
        {
            EmployeeHHMWindow employeesWindow = new EmployeeHHMWindow(Employee);
            employeesWindow.Show();
            this.Hide();
        }
    }
}
