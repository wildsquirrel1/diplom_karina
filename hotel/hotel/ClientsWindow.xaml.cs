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
    /// Логика взаимодействия для ClientsWindow.xaml
    /// </summary>
    public partial class ClientsWindow : Window
    {
        private Employee _employee;
        private List<Clint> _allClients = new();
        private List<Book> _allBookings = new();
        public ClientsWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;
            emplName.Text += $" {employee.Name} {employee.Lastname}";
            emplRole.Text += " " + employee.IdroleNavigation.Name;
            LoadClientsAsync();
        }

        private async void LoadClientsAsync()
        {
            clientsPanel.Children.Clear();

            try
            {
                _allClients = await Api.GetClients();
                _allBookings = await Api.GetBookingsForCurrentHotel(_employee.Idemployee);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Уведомление");
                return;
            }

            DisplayClients(_allClients);
        }

        private void DisplayClients(List<Clint> clientsToShow)
        {
            clientsPanel.Children.Clear();

            var filtered = ApplySearch(clientsToShow, searchTB.Text?.ToLower());
            filtered = ApplySort(filtered, sortBox.SelectedIndex);
            filtered = ApplyFilter(filtered, filterBox.SelectedIndex);

            if (filtered.Count == 0)
            {
                notfound.Visibility = Visibility.Visible;
                scrollViewer.Visibility = Visibility.Collapsed;
            }
            else
            {
                notfound.Visibility = Visibility.Collapsed;
                scrollViewer.Visibility = Visibility.Visible;

                foreach (var client in filtered)
                {
                    var control = new ClientControl(_employee);
                    control.SetData(client, _allBookings);
                    control.OnShowBookings += (s, c) => ShowClientBookings(c);
                    clientsPanel.Children.Add(control);
                }
            }
        }

        private void ShowClientBookings(Clint client)
        {
            var clientBookings = _allBookings.Where(b => b.ClientId == client.Idclint).ToList();
            var bookingsWindow = new ClientBookingsDetailWindow(client, clientBookings, _employee);
            bookingsWindow.ShowDialog();
        }

        private List<Clint> ApplySort(List<Clint> clients, int sortIndex)
        {
            return sortIndex switch
            {
                0 => clients.OrderByDescending(c => c.Idclint).ToList(), 
                1 => clients.OrderBy(c => c.Lastname).ToList(),    
                2 => clients.OrderByDescending(c => c.Lastname).ToList(),
                _ => clients.OrderByDescending(c => c.Idclint).ToList()
            };
        }

        private List<Clint> ApplyFilter(List<Clint> clients, int filterIndex)
        {
            if (filterIndex == 0)
            {
                return clients;
            }

            return clients.Where(c =>
            {
                var clientBookings = _allBookings.Where(b => b.ClientId == c.Idclint).ToList();

                if (filterIndex == 1)
                {
                    return clientBookings.Any(b => b.StatusBook == 1);
                }
                else if (filterIndex == 2)
                {
                    return clientBookings.Any() && clientBookings.All(b => b.StatusBook == 3);
                }
                return true;
            }).ToList();
        }

        private List<Clint> ApplySearch(List<Clint> clients, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return clients;

            return clients.Where(c =>
                c.Lastname.ToLower().Contains(query) ||
                c.Name.ToLower().Contains(query) ||
                c.Patronymic.ToLower().Contains(query) ||
                c.Email.ToLower().Contains(query)
            ).ToList();
        }

        private void filterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (clientsPanel != null)
            {
                DisplayClients(_allClients);
            }
        }

        private void sortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (clientsPanel != null)
            {
                DisplayClients(_allClients);
            }
        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            search.Visibility = string.IsNullOrWhiteSpace(searchTB.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
            if(clientsPanel != null)
            {
                DisplayClients(_allClients);
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
