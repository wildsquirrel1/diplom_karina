using DocumentFormat.OpenXml.Bibliography;
using hotel.Data;
using hotel.Models;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Linq;

namespace hotel
{
    /// <summary>
    /// Логика взаимодействия для AddHotelWindow.xaml
    /// </summary>
    public partial class AddHotelWindow : Window
    {
        private string _city;
        private byte[]? _newPhotoBytes;
        EditHotelWindow EditHotelWindow = new EditHotelWindow();
        private Employee currEmployee;
        private byte[]? _contractPdfBytes; 
        private string _contractFileName = ""; 
        public AddHotelWindow(Employee employee)
        {
            InitializeComponent();
            currEmployee = employee;
            emplName.Text += " " + currEmployee.Name + " " + currEmployee.Lastname;
            emplRole.Text += " " + currEmployee.IdroleNavigation.Name;

            nameOfDocument.Text = "Файл не выбран";

            LoadYandexMaps();
        }

        private void nameT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            EditHotelWindow.Preview(e);
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
            EditHotelWindow.Preview(e);
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
                nameT.Text = $"Простой Комфорт {EditHotelWindow.CapitalizeFirstLetter(city)}";
            }
        }

        private async void addB_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(nameT.Text) || string.IsNullOrEmpty(address.Text) || string.IsNullOrEmpty(_city)
                || string.IsNullOrEmpty(phontT.Text) || string.IsNullOrEmpty(emailT.Text) /*|| starsCB.SelectedIndex == -1*/)
            {
                MessageBox.Show("Заполните пустые поля", "Уведомление");
                return;
            }
            if(!EditHotelWindow.Restrictions(address.Text,phontT.Text, emailT.Text))
            {
                return;
            }

            if (_contractPdfBytes == null || _contractPdfBytes.Length == 0)
            {
                MessageBox.Show("Пожалуйста, загрузите файл договора в формате PDF.", "Уведомление");
                return;
            }

            var newHotel = new Hotel
            {
                Name = nameT.Text,
                Address = address.Text,
                City = _city,
                PhoneNumber = phontT.Text,
                Email = emailT.Text,
                Photo = _newPhotoBytes // может быть null
            };

            string error = await Api.AddHotel(newHotel);
            if (error == null)
            {
                MessageBox.Show("Отель успешно добавлен!", "Уведомление");
                
                this.Close();
            }
            else
            {
                MessageBox.Show($"Ошибка: {error}", "Уведомление");
            }

        }

        private void loadPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.png) | *.jpg; *.png"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _newPhotoBytes = File.ReadAllBytes(dialog.FileName);
                    using var ms = new MemoryStream(_newPhotoBytes);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    photoDisplay.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Уведомление");
                }
            }
        }

        private void phontT_KeyDown(object sender, KeyEventArgs e)
        {
            EditHotelWindow.KeyDownNumber(e);

        }

        private void cityT_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateHotelName();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                    "Все внесенные данные не будут сохранены. Вы уверены, что хотите выйти?",
                    "Уведомление",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );
            if (result == MessageBoxResult.No)
                return;
            else
            {
                this.Close();
            }
        }

        private void pdfFormatDocument_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите файл договора (PDF)",
                Filter = "PDF Files (*.pdf)|*.pdf", 
                CheckFileExists = true
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string extension = System.IO.Path.GetExtension(dialog.FileName).ToLower();
                    if (extension != ".pdf")
                    {
                        MessageBox.Show("Ошибка: Выбран неверный формат файла. Требуется PDF.", "Уведомление");
                        return;
                    }

                    _contractPdfBytes = File.ReadAllBytes(dialog.FileName);

                    _contractFileName = System.IO.Path.GetFileName(dialog.FileName);

                    nameOfDocument.Text = _contractFileName;

                    MessageBox.Show($"Файл \"{_contractFileName}\" успешно загружен.", "Уведомление");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Уведомление");
                    _contractPdfBytes = null;
                    _contractFileName = "";
                    nameOfDocument.Text = "Ошибка загрузки";
                    nameOfDocument.Foreground = Brushes.Red;
                }
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
