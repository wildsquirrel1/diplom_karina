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
        public ClientsWindow(Employee employee)
        {
            InitializeComponent();
        }

        private void filterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void sortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void back_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
