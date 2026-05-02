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
    /// Логика взаимодействия для ClientControl.xaml
    /// </summary>
    public partial class ClientControl : UserControl
    {
        public event EventHandler<Clint> OnShowBookings;
        private Clint _client;
        private List<Book> _clientBookings = new();
        Employee _employee;
        public ClientControl(Employee employee)
        {
            InitializeComponent();
            _employee = employee;
            
        }

        public void SetData(Clint client, List<Book> allBookings)
        {
            _client = client;
            _clientBookings = allBookings.Where(b => b.ClientId == client.Idclint).ToList();

            fioText.Text = $"{client.Lastname} {client.Name} {client.Patronymic}".Trim();
            emailText.Text = $"Электронная почта: {client.Email}";

            passportText.Text = $"Серия и номер паспорта: {client.SeriaPass} {client.NumberPass}";

            // Статистика
            bookingsCountText.Text = $"Броней: {_clientBookings.Count}";

            var lastBooking = _clientBookings.OrderByDescending(b => b.CheckInDate).FirstOrDefault();
            if (lastBooking != null)
            {
                lastBookingText.Text = $"Последняя: {lastBooking.CheckInDate:dd.MM.yyyy}";
            }
            else
            {
                lastBookingText.Text = "Последняя: —";
            }
        }

        private void ShowBookings_Click(object sender, RoutedEventArgs e)
        {
            OnShowBookings?.Invoke(this, _client);
        }
    }
}
