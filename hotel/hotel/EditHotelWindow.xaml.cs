using DocumentFormat.OpenXml.ExtendedProperties;
using hotel.Data;
using hotel.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
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
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;

namespace hotel
{
    /// <summary>
    /// Логика взаимодействия для EditHotelWindow.xaml
    /// </summary>
    public partial class EditHotelWindow : Window
    {
        private Hotel selectedhotel;
        private byte[]? _newPhotoBytes; 
        private List<Employee> _allEmployees = new();
        private List<Employee> _availableManagers = new();
        private Employee _currEmpl;
        public EditHotelWindow(Hotel hotel, Employee employee)
        {
            InitializeComponent();
            selectedhotel = hotel;
            _currEmpl = employee;

            emplName.Text += " " + _currEmpl.Name + " " + _currEmpl.Lastname;
            emplRole.Text += " " + _currEmpl.IdroleNavigation.Name;

            LoadData();

            _newPhotoBytes = hotel.Photo;
            UpdatePhotoDisplay();
        }

        private async void LoadData()
        {
            nameT.Text = selectedhotel.Name;
            address.Text = selectedhotel.Address;
            cityT.Text = selectedhotel.City;
            phoneT.Text = selectedhotel.PhoneNumber;
            emailT.Text = selectedhotel.Email;
            //starsCB.SelectedIndex = selectedhotel.Stars.GetValueOrDefault() - 1;

            _allEmployees = await Api.GetEmployeesForCurrentUser(_currEmpl.Idemployee);

            _availableManagers = _allEmployees
                .Where(e => e.Idhotel == null || e.Idhotel == selectedhotel.Idhotel)
                .ToList();

            mainManager.ItemsSource = _availableManagers.Select(e => $"{e.Lastname} {e.Name} {e.Patronymic}".Trim()).ToList();

            var currentManager = _allEmployees.FirstOrDefault(e => e.Idhotel == selectedhotel.Idhotel);

            if (currentManager != null)
            {
                int index = _availableManagers.FindIndex(m => m.Idemployee == currentManager.Idemployee);

                if (index >= 0)
                {
                    mainManager.SelectedIndex = index;
                }
                else
                {
                    MessageBox.Show("Текущий менеджер не найден в списке доступных.", "Внимание");
                }
            }
            else
            {
                mainManager.SelectedIndex = -1;
            }
        }


            public EditHotelWindow() { }

        private void UpdatePhotoDisplay()
        {
            if (selectedhotel.Photo != null && selectedhotel.Photo.Length > 0)
            {
                try
                {
                    var ms = new MemoryStream(selectedhotel.Photo);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    photoDisplay.Source = bitmap;
                }
                catch
                {
                    photoDisplay.Source = GetDefaultImage();
                }
            }
            else
            {
                photoDisplay.Source = GetDefaultImage();
            }
        }

        public bool Restrictions(string address, string phone, string email)
        {

            if (!isValidAddress(address))
            {
                MessageBox.Show("Адрес должен быть в формате: ул. Название, д. 12 или ул. Название, д. 12/3 или ул. Название, д. 12а", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!IsValidNumber(phone))
            {
                MessageBox.Show("Неправильный формат телефона. Пример: 89123456789", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Неправильный формат электронной почты.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void nameT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Preview(e);
        }

        private void address_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                bool isAllowed =
                    (c >= 'А' && c <= 'Я') ||
                    (c >= 'а' && c <= 'я') ||
                    char.IsDigit(c) ||
                    char.IsWhiteSpace(c) ||
                    c == '.' || c == ',' || c == '-' || c == '/';

                if (!isAllowed)
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void cityT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Preview(e);
        }

        public void Preview(TextCompositionEventArgs e)
        {
            if (!IsRussianLetter(e.Text))
            {
                e.Handled = true;
            }
            if (e.Text == "-")
            {
                e.Handled = false;
            }
        }

        public void KeyDownNumber(KeyEventArgs e)
        {
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                e.Handled = true;
            }
        }

        public bool IsRussianLetter(string text)
        {
            foreach (char c in text)
            {
                if ((c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'))
                {
                    return true;
                }
            }
            return false;
        }

        public string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            char firstChar = char.ToUpper(input[0]);
            string restOfTheString = input.Substring(1).ToLower();

            return firstChar + restOfTheString;
        }

        /*public bool IsUniqueEmail(string email)
        {
            if (selectedhotel != null)
            {
                return !App.context.Hotels.Any(u => u.Email == email && u.Email != selectedhotel.Email);
            }
            return !App.context.Hotels.Any(u => u.Email == email);
        }

        public bool IsUniquePhoneNumber(string number)
        {
            if (selectedhotel != null)
            {
                return !App.context.Hotels.Any(u => u.PhoneNumber == number && u.PhoneNumber != selectedhotel.PhoneNumber);
            }

            return !App.context.Hotels.Any(u => u.PhoneNumber == number);
        }*/

        public bool isValidAddress(string address)
        {
            if(string.IsNullOrEmpty(address) || address.Length > 100)
                return false;
            var regex = new Regex(@"^ул\. [^,\d]+, д\. \d+(?:[/][\dа-яА-ЯёЁ]|[а-яА-ЯёЁ])?$");
            return regex.IsMatch(address);
        }

        public bool IsValidEmail(string email)
        {
            Regex mail = new Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.(ru|com)$");
            if (string.IsNullOrEmpty(email))
                return false;
            if (email.Length > 100) return false;
            if (!mail.IsMatch(email)) return false;
            return true;
        }

        public bool IsValidNumber(string number)
        {
            var regex_for_phone = new Regex(@"^(?:\+7|8)[0-9]{10}$");

            if (string.IsNullOrEmpty(number))
                return false;
            else if (!regex_for_phone.IsMatch(number))
                return false;
            else return true;
        }

        private void phoneT_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDownNumber(e);
        }
        private void UpdateHotelName()
        {
            string city = cityT.Text?.Trim();
            if (string.IsNullOrEmpty(city))
            {
                nameT.Text = "Simple Comfort";
            }
            else
            {
                nameT.Text = $"Simple Comfort {CapitalizeFirstLetter(city)}";
            }
        }

        private async void edit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(nameT.Text) || string.IsNullOrEmpty(address.Text) || string.IsNullOrEmpty(cityT.Text)
                || string.IsNullOrEmpty(phoneT.Text) || string.IsNullOrEmpty(emailT.Text) ||  mainManager.SelectedIndex == -1)
            {
                MessageBox.Show("Заполните пустые поля", "Уведомление");
                return;
            }
            if (Restrictions(address.Text, phoneT.Text, emailT.Text))
            {
                Employee selectedManager = null;
                if (mainManager.SelectedIndex != -1)
                {
                    selectedManager = _availableManagers[mainManager.SelectedIndex];
                }

                string error = await EditHotel(nameT.Text, address.Text, cityT.Text, phoneT.Text, emailT.Text);
                if (error == null)
                {
                    if (selectedManager != null)
                    {
                        selectedManager.Idhotel = selectedhotel.Idhotel;

                        var updateEmployeeError = await Api.UpdateEmployee(selectedManager.Idemployee, selectedManager);
                        if (updateEmployeeError != null)
                        {
                            MessageBox.Show($"Ошибка при обновлении менеджера: {updateEmployeeError}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    MessageBox.Show("Отель успешно отредактирован!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Ошибка: {error}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ResetParams()
        {
            nameT.Text = string.Empty;
            address.Text = string.Empty;
            cityT.Text = string.Empty;
            phoneT.Text = string.Empty;
            emailT.Text = string.Empty;
            //starsCB.SelectedIndex = 0;
        }

        private async Task<string> EditHotel(string name, string address, string city, string phone, string email)
        {
            selectedhotel.Name = name;
            selectedhotel.Address = address;
            selectedhotel.City = city;
            selectedhotel.PhoneNumber = phone;
            selectedhotel.Email = email;
            //selectedhotel.Stars = stars;
            selectedhotel.Photo = _newPhotoBytes;

            return await Api.updateHotel(selectedhotel.Idhotel, selectedhotel);
        }

        private void cityT_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateHotelName();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            bool hasUnsavedChanges =
            nameT.Text != selectedhotel.Name ||
            address.Text != selectedhotel.Address ||
            cityT.Text != selectedhotel.City ||
            phoneT.Text != selectedhotel.PhoneNumber ||
            emailT.Text != selectedhotel.Email;
            //starsCB.SelectedIndex != selectedhotel.Stars - 1;

            if (hasUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show(
                    "У вас есть несохраненные изменения. Вы уверены, что хотите выйти?",
                    "Уведомление",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.No)
                    return;
            }

            this.Close();
        }

        private ImageSource GetDefaultImage()
        {
            try
            {
                return new BitmapImage(new Uri("pack://application:,,,/Images/default-hotel.jpg"));
            }
            catch
            {
                return null;
            }
        }

        private void deleteB_Click(object sender, RoutedEventArgs e)
        {
            _newPhotoBytes = null; 
            photoDisplay.Source = GetDefaultImage();
        }

        private void editPhotoB_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.png) | *.jpg; *.png"
            };
            if (dialog.ShowDialog() == true)
            {
                _newPhotoBytes = File.ReadAllBytes(dialog.FileName); 
                photoDisplay.Source = LoadBitmap(_newPhotoBytes); 
            }
        }
        private ImageSource LoadBitmap(byte[]? imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return GetDefaultImage();

            try
            {
                using var ms = new MemoryStream(imageData);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return GetDefaultImage();
            }
        }
    }
}
