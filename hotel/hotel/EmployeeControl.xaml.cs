using DocumentFormat.OpenXml.Vml.Office;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hotel
{
    /// <summary>
    /// Логика взаимодействия для EmployeeControl.xaml
    /// </summary>
    public partial class EmployeeControl : UserControl
    {
        public Employee Employee { get; set; }
        public Employee CurrentUser { get; set; }
        public Action OnEmployeeUpdated { get; set; }
        public List<Role> Roles { get; set; }
        public List<Hotel> Hotels { get; set; }
        public EmployeeControl(Employee emp, Employee currentUser, List<Role> roles, List<Hotel> hotels)
        {
            InitializeComponent();
            Employee = emp;
            Roles = roles;
            Hotels = hotels;
            UpdateDisplay();
            CurrentUser = currentUser; 
        }

        private void UpdateDisplay()
        {
            fullNameBlock.Text = $"{Employee.Lastname} {Employee.Name} {Employee.Patronymic}".Trim();

            /*var role = Roles.FirstOrDefault(r => r.Idrole == Employee.Idrole);
            roleBlock.Text = role?.Name ?? "Не указана";*/

            var hotel = Hotels.FirstOrDefault(h => h.Idhotel == Employee.Idhotel);
            hotelBlock.Text = hotel?.Name ?? "Не привязан";

            phoneBlock.Text = $"Телефон: {Employee.PhoneNumber}";
            emailBlock.Text = $"Email: {Employee.Email}";

            if (Employee.Status == 1)
            {
                statusBlock.Text = "(Уволен)";
                statusBlock.Visibility = Visibility.Visible;
                Fire.Content = "Восстановить";
                Fire.Background = new SolidColorBrush(System.Windows.Media.Colors.LightGreen);
            }
            else
            {
                statusBlock.Visibility = Visibility.Collapsed;
                Fire.Content = "Уволить";
                Fire.Background = new SolidColorBrush(System.Windows.Media.Colors.LightCoral);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditEmployeeWindow(Employee, CurrentUser);
            if (editWindow.ShowDialog() == true)
            {
                UpdateDisplay();
                OnEmployeeUpdated?.Invoke();
            }
        }

        private async void Fire_Click(object sender, RoutedEventArgs e)
        {
            string action = Employee.Status == 0 ? "уволить" : "восстановить";
            string action2 = Employee.Status == 0 ? "уволен" : "восстановлен";
            string message = $"Вы действительно хотите {action} сотрудника\n{fullNameBlock.Text}?";

            var result = MessageBox.Show(message, "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
                return;

            Employee.Status = (sbyte)(Employee.Status == 0 ? 1 : 0);

            string error = await Api.UpdateEmployee(Employee.Idemployee, Employee);
            if (error == null)
            {   
                MessageBox.Show($"Сотрудник успешно {action2}!", "Уведомление", MessageBoxButton.OK);
                UpdateDisplay();
            }
            else
            {
                MessageBox.Show($"Ошибка: {error}", "Уведомление", MessageBoxButton.OK);
                Employee.Status = (sbyte)(Employee.Status == 1 ? 0 : 1);
                UpdateDisplay();
            }
        }
    }
}
