using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.ExtendedProperties;
using hotel.Data;
using hotel.Models;
using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
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
        private string _city;
        private Hotel selectedhotel;
        private byte[]? _newPhotoBytes;
        private List<Employee> _managerComboItems = new();
        private bool _headManagerAssigned;
        private int? _initialHeadManagerId;
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

            LoadYandexMaps();
        }

        public EditHotelWindow() { }

        private async void LoadData()
        {
            nameT.Text = selectedhotel.Name;
            address.Text = selectedhotel.Address;
            _city = selectedhotel.City ?? "Нет города";
            phoneT.Text = selectedhotel.PhoneNumber;
            emailT.Text = selectedhotel.Email;
            //starsCB.SelectedIndex = selectedhotel.Stars.GetValueOrDefault() - 1;

            var currentManager = await Api.GetHotelHeadManager(selectedhotel.Idhotel);
            _initialHeadManagerId = currentManager?.Idemployee;
            _headManagerAssigned = currentManager != null;

            if (_headManagerAssigned)
            {
                _managerComboItems = new List<Employee> { currentManager! };
            }
            else
            {
                _managerComboItems = await Api.GetFreeHeadManagers();
            }

            mainManager.ItemsSource = _managerComboItems.Select(FormatManagerName).ToList();
            mainManager.IsEnabled = !_headManagerAssigned;
            mainManager.SelectedIndex = _managerComboItems.Count > 0 ? 0 : -1;
        }

        private static string FormatManagerName(Employee employee) =>
            $"{employee.Lastname} {employee.Name} {employee.Patronymic}".Trim();

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

            /*if (!isValidAddress(address))
            {
                MessageBox.Show("Адрес должен быть в формате: ул. Название, или ул. Название, д. 12/3 или ул. Название, д. 12а", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }*/

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

        /*private void address_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
        }*/

        /*private void cityT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Preview(e);
        }*/

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
        }

        public bool isValidAddress(string address)
        {
            if (string.IsNullOrEmpty(address) || address.Length > 100)
                return false;

            //var regex = new Regex(@"^улица\. [^,\d]+, д\. \d+(?:[/][\dа-яА-ЯёЁ]|[а-яА-ЯёЁ])?$");

            var regex = new Regex(@"^улица\. [^,\d]+, \d+(?:[/][\dа-яА-ЯёЁ]|[а-яА-ЯёЁ])?$");

            return regex.IsMatch(address);
        }*/

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
            string city = _city.Trim();
            if (string.IsNullOrEmpty(city))
            {
                nameT.Text = "Простой Комфорт";
            }
            else
            {
                nameT.Text = $"Простой Комфорт {CapitalizeFirstLetter(city)}";
            }
        }

        private async void edit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(nameT.Text) || string.IsNullOrEmpty(address.Text) || string.IsNullOrEmpty(_city)
                || string.IsNullOrEmpty(phoneT.Text) || string.IsNullOrEmpty(emailT.Text))
            {
                MessageBox.Show("Заполните пустые поля", "Уведомление");
                return;
            }

            if (!_headManagerAssigned && _managerComboItems.Count > 0 && mainManager.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите главного менеджера", "Уведомление");
                return;
            }

            if (Restrictions(address.Text, phoneT.Text, emailT.Text))
            {
                Employee? selectedManager = mainManager.SelectedIndex >= 0
                    ? _managerComboItems[mainManager.SelectedIndex]
                    : null;

                string error = await EditHotel(nameT.Text, address.Text, _city, phoneT.Text, emailT.Text);
                if (error == null)
                {
                    var managerError = await AssignHeadManagerAsync(selectedManager);
                    if (managerError != null)
                    {
                        MessageBox.Show($"Ошибка при обновлении менеджера: {managerError}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
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

        private async Task<string?> AssignHeadManagerAsync(Employee? selectedManager)
        {
            if (_headManagerAssigned || selectedManager == null)
                return null;

            if (selectedManager.Idhotel != selectedhotel.Idhotel)
            {
                selectedManager.Idhotel = selectedhotel.Idhotel;
                return await Api.UpdateEmployee(selectedManager.Idemployee, selectedManager);
            }

            return null;
        }

        private void ResetParams()
        {
            nameT.Text = string.Empty;
            address.Text = string.Empty;
            _city = string.Empty;
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

        /*private void cityT_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateHotelName();
        }*/

        private void back_Click(object sender, RoutedEventArgs e)
        {
            int? selectedManagerId = mainManager.SelectedIndex >= 0
                ? _managerComboItems[mainManager.SelectedIndex].Idemployee
                : null;

            bool hasUnsavedChanges =
            nameT.Text != selectedhotel.Name ||
            address.Text != selectedhotel.Address ||
            _city != selectedhotel.City ||
            phoneT.Text != selectedhotel.PhoneNumber ||
            emailT.Text != selectedhotel.Email ||
            selectedManagerId != _initialHeadManagerId;
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
                return new BitmapImage(new Uri("pack://application:,,,/images/a-none-logo.jpg"));
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

        // Загрузка Яндекс карты
        private async void LoadYandexMaps()
        {
            try
            {
                await YandexMapWebView.EnsureCoreWebView2Async();
                System.Diagnostics.Debug.WriteLine("WebView2 инициализирован");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка WebView2: {ex.Message}");
                return;
            }

            await YandexMapWebView.EnsureCoreWebView2Async();

            // HTML со встроенной картой
            string html = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <script src='https://api-maps.yandex.com/2.1/?apikey=e0855f9d-43db-40c1-bccd-c33341fdf936&lang=ru_RU'></script>
                    <style>
                        html, body, #map { width: 100%; height: 100%; margin: 0; padding: 0; }
                    </style>
                </head>
                <body>
                    <div id='map'></div>
                    <script>
                        var myMap, myPlacemark;

                        ymaps.ready(init);

                        function init() {
                            myMap = new ymaps.Map('map', {
                                center: [55.76, 37.64],
                                zoom: 10,
                                controls: ['zoomControl', 'searchControl']
                            });
            
                            // Клик по карте
                            myMap.events.add('click', function (e) {
                                var coords = e.get('coords');
                                setPlacemark(coords);
                
                                ymaps.geocode(coords).then(function (res) {
                                    var obj = res.geoObjects.get(0);
                                    sendToCSharp(obj);
                                });
                            });
                        }

                        function setPlacemark(coords) {
                            if (myPlacemark) myMap.geoObjects.remove(myPlacemark);
                            myPlacemark = new ymaps.Placemark(coords);
                            myMap.geoObjects.add(myPlacemark);
                        }

                        // Поиск по адресу из C#
                        function searchAddress(query) {
                            ymaps.geocode(query).then(function (res) {
                                var obj = res.geoObjects.get(0);
                                if (!obj) {
                                    window.chrome.webview.postMessage(JSON.stringify({error: 'Не найдено'}));
                                    return;
                                }
                                var coords = obj.geometry.getCoordinates();
                                myMap.setCenter(coords, 16);
                                setPlacemark(coords);
                                sendToCSharp(obj);
                            });
                        }

                        // Отправка данных в C#
                        function sendToCSharp(geoObject) {
                            var data = {
                                address: geoObject.getAddressLine(),
                                city: geoObject.getLocalities().join(', ') || '',
                                country: geoObject.getCountry() || '',
                                coords: geoObject.geometry.getCoordinates()
                            };
                            window.chrome.webview.postMessage(JSON.stringify(data));
                        }
                    </script>
                </body>
                </html>";

            YandexMapWebView.NavigateToString(html);

            // Обработка сообщений из JavaScript
            YandexMapWebView.WebMessageReceived += OnWebMessageReceived;
        }

        private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                string json = e.TryGetWebMessageAsString();

                // Проверка на ошибку
                if (json.Contains("\"error\""))
                {
                    address.Text = "Место не найдено";
                    _city = "Место не найдено";
                    return;
                }

                // Парсим JSON
                var data = System.Text.Json.JsonSerializer.Deserialize<AddressData>(json);

                if (data != null)
                {
                    // Форматируем вывод: Город | Полный адрес
                    address.Text = data.address;
                    _city = data.city;
                }
            });
        }
    }
}

// Не менять!
public class AddressData
{
    public string address { get; set; } = "";
    public string city { get; set; } = "";
    public string country { get; set; } = "";
    public double[] coords { get; set; } = Array.Empty<double>();
}