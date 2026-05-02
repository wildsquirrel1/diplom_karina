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
    /// Логика взаимодействия для ServiceControl.xaml
    /// </summary>
    public partial class ServiceControl : UserControl
    {
        private Service _service;
        public Action OnServiceUpdated { get; set; }
        Employee _employee;
        public ServiceControl(Employee employee)
        {
            InitializeComponent();
            _employee = employee;
        }

        public void SetService(Service services)
        {
            _service = services;
            serviceNameText.Text = services.Name;

            descriptionText.Text = services.Description.Length > 100 ? services.Description.Substring(0, 97) + "..." : services.Description;

            costText.Text = $"{services.Cost:N0} ₽";

            SetStatus(services.Status);
        }
        private void SetStatus(sbyte? status)
        {
            var converter = new BrushConverter();

            if (status == 1)
            {
                statusBadge.Background = (Brush)converter.ConvertFrom("#E8F5E9");
                statusText.Text = "Активно";
                statusText.Foreground = (Brush)converter.ConvertFrom("#2E7D32");
                cardBorder.Opacity = 1;
            }
            else
            {
                statusBadge.Background = (Brush)converter.ConvertFrom("#FFEBEE");
                statusText.Text = "Неактивно";
                statusText.Foreground = (Brush)converter.ConvertFrom("#C62828");
                cardBorder.Opacity = 0.6;
            }
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditServiceWindow(_employee,_service);
            if (editWindow.ShowDialog() == true)
            {
                
                OnServiceUpdated?.Invoke();
            }
        }
    }
}
