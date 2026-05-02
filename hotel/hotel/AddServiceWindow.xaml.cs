using hotel.Data;
using hotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddServiceWindow.xaml
    /// </summary>
    public partial class AddServiceWindow : Window
    {
        private Employee _employee;
        public AddServiceWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;

            emplName.Text += $" {employee.Name} {employee.Lastname}";
            emplRole.Text += " " + employee.IdroleNavigation.Name;

            statusCB.Items.Add("Активна");
            statusCB.Items.Add("Не активна");
            statusCB.SelectedIndex = 0;
        }

        private async void addB_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTB.Text) ||
                string.IsNullOrWhiteSpace(priceTB.Text) ||
                statusCB.SelectedIndex == -1)
            {
                MessageBox.Show("Заполните все обязательные поля", "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateInput(nameTB.Text, descriptionTB, priceTB))
                return;

            var newService = new Service
            {
                Name = nameTB.Text.Trim(),
                Description = descriptionTB.Text.Trim(),
                Cost = decimal.Parse(priceTB.Text.Replace(',', '.')),
                Status = (sbyte)(statusCB.SelectedIndex == 0 ? 1 : 0)
            };

            try
            {
                var error = await Api.AddService(newService);

                if (error == null)
                {
                    MessageBox.Show("Услуга успешно добавлена!", "Уведомление",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show($"Ошибка: {error}", "Уведомление",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка соединения: {ex.Message}", "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool IsDigitOrDot(string text)
        {
            foreach (char c in text)
            {
                if (char.IsDigit(c) || c == '.' || c == ',')
                    return true;
            }
            return false;
        }
        private bool IsRussianLetterOrHyphen(string text)
        {
            foreach (char c in text)
            {
                if ((c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я') || c == '-' || c == ' ')
                    return true;
            }
            return false;
        }

        private void priceTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDigitOrDot(e.Text);
        }

        private void descriptionTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\n" || e.Text == "\r")
                e.Handled = true;
        }

        private void nameTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsRussianLetterOrHyphen(e.Text);
        }

        private bool ValidateInput(string name, TextBox description, TextBox price)
        {
            if (!IsValidServiceName(name))
            {
                MessageBox.Show("Название должно содержать только кириллицу, дефисы и пробелы (2-50 символов)",
                    "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                nameTB.Focus();
                return false;
            }

            if (description.Text.Length > 200)
            {
                MessageBox.Show("Описание не должно превышать 200 символов",
                    "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                description.Focus();
                return false;
            }

            if (!decimal.TryParse(price.Text.Replace(',', '.'), out decimal parsedPrice) || parsedPrice <= 0)
            {
                MessageBox.Show("Введите корректную положительную стоимость",
                    "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                priceTB.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidServiceName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 2 || name.Length > 50)
                return false;

            var regex = new Regex(@"^[А-Яа-яЁё\s\-]+$");
            return regex.IsMatch(name);
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
