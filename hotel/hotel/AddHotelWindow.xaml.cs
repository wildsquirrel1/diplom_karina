using hotel.Data;
using hotel.Models;
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
            string city = cityT.Text?.Trim();
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
            if(string.IsNullOrEmpty(nameT.Text) || string.IsNullOrEmpty(address.Text) || string.IsNullOrEmpty(cityT.Text)
                || string.IsNullOrEmpty(phontT.Text) || string.IsNullOrEmpty(emailT.Text) || starsCB.SelectedIndex == -1)
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
                City = cityT.Text,
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
    }
}
