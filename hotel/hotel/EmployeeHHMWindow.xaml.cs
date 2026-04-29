using hotel.Data;
using hotel.Models;
using System;
using System.Collections;
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
    /// Логика взаимодействия для EmployeeHHMWindow.xaml
    /// </summary>
    public partial class EmployeeHHMWindow : Window
    {
        private Employee _currentUser;
        private List<Employee> _allEmployees = new();

        public EmployeeHHMWindow(Employee currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            emplName.Text += " " + currentUser.Name + " " + currentUser.Lastname;
            emplRole.Text += " " + currentUser.IdroleNavigation.Name;
            LoadEmployees();
        }

        private async void LoadEmployees()
        {
            employeesPanel.Children.Clear();
            var roles = await Api.GetRoles();
            var hotels = await Api.GetHotels();

            var employees = await Api.GetEmployeesForCurrentUser(_currentUser.Idemployee);
            _allEmployees = employees;

            var filtered = ApplySearch(_allEmployees, searchTB.Text?.ToLower());
            filtered = ApplySort(filtered, sortBox.SelectedIndex);
            filtered = ApplyFilter(filtered, filterBox.SelectedIndex);

            foreach (var emp in filtered)
            {
                var control = new EmployeeControl(emp, _currentUser, roles, hotels) { OnEmployeeUpdated = LoadEmployees };
                employeesPanel.Children.Add(control);
            }

            if (employeesPanel.Children.Count == 0)
            {
                notfound.Visibility = Visibility.Visible;
                stack.Visibility = Visibility.Collapsed;
            }
            else
            {
                notfound.Visibility = Visibility.Collapsed;
                stack.Visibility = Visibility.Visible;
            }
        }

        private List<Employee> ApplySearch(List<Employee> employeelist, string search)
        {
            if(!string.IsNullOrEmpty(search))
            {
                employeelist = employeelist.Where(h => h.Name.ToLower().Contains(search)).ToList();
            }
            return employeelist;
        }

        private List<Employee> ApplyFilter(List<Employee> list, int index)
        {
            return index switch
            {
                0 => list,
                1 => list.Where(e => e.Status == 0).ToList(),
                2 => list.Where(e => e.Status == 1).ToList(),
                _ => list
            };
        }
        private List<Employee> ApplySort(List<Employee> list, int index)
        {
            return index switch
            {
                0 => list.OrderByDescending(e => e.Idemployee).ToList(),
                1 => list.OrderBy(e => e.Lastname).ToList(),
                2 => list.OrderByDescending(e => e.Lastname).ToList(),
                _ => list.OrderByDescending(e => e.Idemployee).ToList()
            };
        }

        public void NotFound()
        {
            if (employeesPanel.Children.Count == 0 || employeesPanel == null)
            {
                notfound.Visibility = Visibility.Visible;
                stack.Visibility = Visibility.Collapsed;
            }
            else
            {
                notfound.Visibility = Visibility.Collapsed;
                stack.Visibility = Visibility.Visible;
            }
        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchTB.Text))
            {
                search.Visibility = Visibility.Hidden;
            }
            else
            {
                search.Visibility = Visibility.Hidden;
            }
            if (employeesPanel != null)
            {
                LoadEmployees();
                NotFound();
            }
        }

        private void sortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (employeesPanel != null)
            {
                LoadEmployees();
                NotFound();
            }
        }

        private void filterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (employeesPanel != null)
            {
                LoadEmployees();
                NotFound();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEmployeeWindow(_currentUser);
            if (addWindow.ShowDialog() == true)
            {
                LoadEmployees();
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            if(_currentUser.Idrole == 1)
            {
                HeadHotelManagerWindow headHotelManagerWindow = new HeadHotelManagerWindow(_currentUser);
                headHotelManagerWindow.Show();
            }
            if(_currentUser.Idrole == 2)
            {
                MainManager mainManager = new MainManager(_currentUser);
                mainManager.Show();
            }
        }
    }
}
