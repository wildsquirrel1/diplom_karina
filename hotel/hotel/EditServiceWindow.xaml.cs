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
    /// Логика взаимодействия для EditServiceWindow.xaml
    /// </summary>
    public partial class EditServiceWindow : Window
    {
        private Service _service;
        public EditServiceWindow(Employee employee, Service service)
        {
            InitializeComponent();
            _service = service;

            nameTB.Text = service.Name;
            descriptionTB.Text = service.Description;
            priceTB.Text = service.Cost.ToString();

            statusCB.Items.Clear();
            statusCB.Items.Add("Активна");
            statusCB.Items.Add("Не активна");
            statusCB.SelectedIndex = (service.Status ?? 0) == 1 ? 0 : 1;
        }

        private void descriptionTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\n" || e.Text == "\r")
                e.Handled = true;
        }

        private void priceTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDigitOrDot(e.Text);
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

        private bool IsDigitOrDot(string text)
        {
            foreach (char c in text)
            {
                if (char.IsDigit(c) || c == '.' || c == ',')
                    return true;
            }
            return false;
        }

        private async void editB_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTB.Text) ||
                string.IsNullOrWhiteSpace(priceTB.Text) ||
                statusCB.SelectedIndex == -1)
            {
                MessageBox.Show("Заполните все обязательные поля", "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 🔹 2. Валидация данных
            if (!ValidateInput(nameTB.Text, descriptionTB, priceTB))
                return;

            // 🔹 3. Обновляем объект услуги
            _service.Name = nameTB.Text.Trim();
            _service.Description = descriptionTB.Text.Trim();
            _service.Cost = decimal.Parse(priceTB.Text.Replace(',', '.'));
            _service.Status = (sbyte)(statusCB.SelectedIndex == 0 ? 1 : 0);

            try
            {
                // 🔹 4. Отправляем на сервер
                var error = await Api.UpdateService(_service);

                if (error == null)
                {
                    MessageBox.Show("Услуга успешно отредактирована!", "Уведомление",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;  // ← Возвращаем успех для делегата
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

        private bool ValidateInput(string name, TextBox description, TextBox prices )
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

            if (!decimal.TryParse(prices.Text.Replace(',', '.'), out decimal price) || price <= 0)
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

        private void nameTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsRussianLetterOrHyphen(e.Text);
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
