using hotel_demo.Model;
using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private Employeer selectedEmployee;
        
        public AdminWindow()
        {
            InitializeComponent();
            App.contex.Employeers.Include(e => e.Role).Load();
            App.contex.Roles.ToList();


            foreach(var role in App.contex.Roles)
            {
                filterCB.Items.Add(role.Name);
            }
            
            updateDataGrid();
        }
        private void updateDataGrid()
        {
            App.contex.Employeers.Load();

            EmployeeDG.Items.Refresh();
            EmployeeDG.ItemsSource = App.contex.Employeers.Local.ToBindingList();

            var emp = App.contex.Employeers.ToList();
            emp = ApplySearch(emp, searchTB.Text);
            emp = ApplySort(emp, sortCB.SelectedIndex);
            emp = ApplyFilter(emp, filterCB.SelectedValue.ToString(), filterCB.SelectedIndex);

            EmployeeDG.ItemsSource = emp;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Autoriz autoriz = new Autoriz();
            autoriz.Show();
        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(EmployeeDG != null)
            {
                updateDataGrid();
            }
        }

        private List<Employeer> ApplySearch(List<Employeer> employees, string search)
        {
            if(!string.IsNullOrEmpty(search))
            {
                employees = employees.Where(e => e.Name.ToLower().StartsWith(search) || e.Lastname.ToLower().StartsWith(search) || e.Patronymic.ToLower().StartsWith(search)).ToList();
            }
            return employees;
        }

        private List<Employeer> ApplySort(List<Employeer> employeers, int sortIndex)
        {
            switch (sortIndex)
            {
                case 0:
                    employeers = employeers.OrderBy(d => d.Lastname).ToList(); break;
                case 1:
                    employeers = employeers.OrderByDescending(d => d.Lastname).ToList(); break;
            }
            return employeers;
        }

        private List<Employeer> ApplyFilter (List<Employeer> employeers, string role, int index)
        {
            App.contex.Roles.ToList();

            if(index == 0) return employeers;

            employeers = employeers.Where(i => i.Role.Name == role).ToList();
            return employeers;
        }
        private void EmployeeDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(EmployeeDG.SelectedItem != null)
            {
                selectedEmployee = (Employeer)EmployeeDG.SelectedItem;
                if(selectedEmployee == null)
                {
                    MessageBox.Show("Поле пустое", "Ошибка");
                    return;
                }
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            
            AddWindow add = new AddWindow();
            this.Hide();
            add.ShowDialog();
            updateDataGrid();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            if(EmployeeDG.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования!!", "Ошибка");
                return;
            }
            else
            {
                EditWindow editWindow = new EditWindow(selectedEmployee);
                this.Hide();
                editWindow.ShowDialog();
                updateDataGrid();
            }
            updateDataGrid();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updateDataGrid();
        }

        private void sortCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeDG != null)
            {
                updateDataGrid();
            }
        }

        private void filterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeDG != null)
            {
                updateDataGrid();
            }
        }

        private void statusEdit_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDG.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования!!", "Ошибка");
                return;
            }
            else
            {
                selectedEmployee.Isblocked = false;
                App.contex.SaveChanges();
            }
            updateDataGrid();
        }

       /* private void EmployeeDG_Loaded(object sender, RoutedEventArgs e)
        {
            if (isblockedItem.)
            {
                isblockedItem.ToString();
            }

        }*/
    }
}
