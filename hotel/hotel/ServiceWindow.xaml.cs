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
    /// Логика взаимодействия для ServiceWindow.xaml
    /// </summary>
    public partial class ServiceWindow : Window
    {
        private Employee _employee;
        private List<Service> _allServices = new();
        public ServiceWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;
            emplName.Text += $" {employee.Name} {employee.Lastname}";
            emplRole.Text += " " + employee.IdroleNavigation.Name;
            LoadServicesAsync();
        }

        private async void LoadServicesAsync() => await ReloadServicesAsync();

        private async Task ReloadServicesAsync()
        {
            try
            {
                _allServices = await Api.GetServices();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки услуг: {ex.Message}", "Уведомление");
                return;
            }

            DisplayServices(_allServices);
        }

        private void DisplayServices(List<Service> servicesToShow)
        {
            serviceList.Children.Clear();

            var filtered = ApplySearch(servicesToShow, searchTB.Text?.ToLower());
            filtered = ApplySort(filtered, sortBox.SelectedIndex);
            filtered = ApplyFilter(filtered, filterBox.SelectedIndex);

            if (filtered.Count == 0)
            {
                notfound.Visibility = Visibility.Visible;
                stack.Visibility = Visibility.Collapsed;
            }
            else
            {
                notfound.Visibility = Visibility.Collapsed;
                stack.Visibility = Visibility.Visible;

                foreach (var service in filtered)
                {
                    var control = new ServiceControl(_employee);
                    control.SetService(service);
                    control.OnServiceUpdated = () => _ = ReloadServicesAsync();
                    serviceList.Children.Add(control);
                }
            }
        }

        private List<Service> ApplySearch(List<Service> services, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return services;

            return services.Where(s =>
                s.Name.ToLower().Contains(query) ||
                s.Description.ToLower().Contains(query)
            ).ToList();
        }

        private List<Service> ApplySort(List<Service> services, int sortIndex)
        {
            return sortIndex switch
            {
                0 => services.OrderByDescending(s => s.Idservice).ToList(),
                1 => services.OrderBy(s => s.Name).ToList(),
                2 => services.OrderByDescending(s => s.Name).ToList(),
                _ => services.OrderByDescending(s => s.Idservice).ToList()
            };
        }

        private List<Service> ApplyFilter(List<Service> services, int filterIndex)
        {
            return filterIndex switch
            {
                0 => services,
                1 => services.Where(s => s.Status == 1).ToList(),
                2 => services.Where(s => s.Status == 0).ToList(),
                _ => services
            };
        }


        private void filterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (serviceList != null)
            {
                DisplayServices(_allServices);
            }
        }

        private void sortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (serviceList != null)
            {
                DisplayServices(_allServices);
            }
        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchTB.Text))
                search.Visibility = Visibility.Visible;
            else
                search.Visibility = Visibility.Hidden;

            if (serviceList != null)
            {
                DisplayServices(_allServices);
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void addService_Click(object sender, RoutedEventArgs e)
        {
            var addService = new AddServiceWindow(_employee);
            if (addService.ShowDialog() == true)
            {
                _allServices.Clear();
                LoadServicesAsync();
            }  
        }
    }
}
