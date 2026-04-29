using hotel.Data;
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
    /// Логика взаимодействия для AddClintWindow.xaml
    /// </summary>
    public partial class AddClintWindow : Window
    {
        private Employee Employee;
        EditEmployeeWindow EditEmployeeWindow;
        private List<Guest> _tempGuests = new List<Guest>();
        public AddClintWindow(Employee employee)
        {
            InitializeComponent();
            Employee = employee;
            emplName.Text += " " + employee.Name + " " + employee.Lastname;
            emplRole.Text += " " + employee.IdroleNavigation.Name;
            EditEmployeeWindow = new EditEmployeeWindow();

            UpdateGuestCount();
        }

        private void UpdateGuestCount()
        {
            countGuests.Text = $"Кол-во гостей: {_tempGuests.Count}";
        }

        private void addGuestButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fioBoxGuest.Text))
            {
                MessageBox.Show("Введите ФИО гостя", "Уведомление");
                return;
            }

            if (string.IsNullOrWhiteSpace(docNumber.Text))
            {
                MessageBox.Show("Введите номер документа", "Уведомление");
                return;
            }

            var guestParts = SplitFio(fioBoxGuest.Text);
            if (guestParts.Length < 2 || guestParts.Length > 3)
            {
                MessageBox.Show("ФИО гостя должно состоять из Фамилии и Имени (и Отчества)", "Уведомление");
                return;
            }

            if (!EditEmployeeWindow.IsValidFI(guestParts[0]))
            {
                MessageBox.Show("Фамилия гостя должна содержать только кириллицу и дефис", "Уведомление");
                return;
            }

            if (!EditEmployeeWindow.IsValidFI(guestParts[1]))
            {
                MessageBox.Show("Имя гостя должно содержать только кириллицу и дефис", "Уведомление");
                return;
            }

            if (guestParts.Length == 3 && !EditEmployeeWindow.IsValidPatr(guestParts[2]))
            {
                MessageBox.Show("Отчество гостя должно содержать только кириллицу и дефис", "Уведомление");
                return;
            }

            int docType = 1;
            if (docTypeCombo != null)
            {
                docType = docTypeCombo.SelectedIndex + 1;
            }

            var newGuest = new Guest
            {
                Lastname = GetLastNameFromFio(fioBoxGuest.Text),
                Name = GetNameFromFio(fioBoxGuest.Text),
                Patronymic = GetPatronymicFromFio(fioBoxGuest.Text),
                DocumentType = (sbyte)docType,
                DocumentNumber = docNumber.Text.Trim()
            };

            if (_tempGuests.Any(g => g.DocumentNumber == newGuest.DocumentNumber))
            {
                MessageBox.Show("Гость с таким номером документа уже добавлен в список!", "Уведомление");
                return;
            }

            _tempGuests.Add(newGuest);

            UpdateGuestCount();

            fioBoxGuest.Clear();
            docNumber.Clear();
            if (FindName("docTypeCombo") is ComboBox cbClear) cbClear.SelectedIndex = 0;

            MessageBox.Show("Гость добавлен в список", "Уведомление");
        }

        private async void doneButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(fioBox.Text) || datebirth.SelectedDate == null || string.IsNullOrEmpty(passportBox.Text)||
                string.IsNullOrEmpty(emailBox.Text) || string.IsNullOrEmpty(passwordBox.Password))
            {
                MessageBox.Show("Заполните пустые поля у клиента", "Уведомление");
                return;
            }
            if (Restrictions(fioBox.Text))
            {
                var clientParts = SplitFio(fioBox.Text);
                var newClient = new Clint
                {
                    Lastname = clientParts[0],
                    Name = clientParts[1],
                    Patronymic = clientParts.Length > 2 ? clientParts[2] : "",
                    Birth = DateOnly.FromDateTime(datebirth.SelectedDate.Value),
                    SeriaPass = passportBox.Text.Length >= 4 ? passportBox.Text.Substring(0, 4) : "",
                    NumberPass = passportBox.Text.Length > 4 ? passportBox.Text.Substring(4) : "",
                    Email = emailBox.Text,
                    Password = passwordBox.Password
                };

                try
                {
                    var createdClient = await Api.AddClient(newClient);

                    if (createdClient == null)
                    {
                        MessageBox.Show("Ошибка при сохранении клиента. Возможно, электронная почта уже занят.", "Уведомление");
                        return;
                    }

                    if (_tempGuests.Count > 0)
                    {
                        foreach (var guest in _tempGuests)
                        {

                            var guestResult = await Api.AddGuest(guest);
                            if (guestResult == null)
                            {
                                MessageBox.Show($"Ошибка при добавлении гостя {guest.Name}. Проверьте данные или соединение.", "Уведомление");
                                return;
                            }
                        }
                    }

                    MessageBox.Show("Клиент и гости успешно сохранены!", "Уведомление");
                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Уведомление");
                }
            }
        }
        private string[] SplitFio(string fio)
        {
            return fio.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void fioBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            EditEmployeeWindow.Preview(e);
        }

        private void fioBoxGuest_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            EditEmployeeWindow.Preview(e);
        }

        private void datebirth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !EditEmployeeWindow.IsTextAllowed(e.Text);
        }

        private void passportBox_KeyDown(object sender, KeyEventArgs e)
        {
            EditEmployeeWindow.KeyDownNumber(e);
        }

        private void docNumber_KeyDown(object sender, KeyEventArgs e)
        {
            EditEmployeeWindow.KeyDownNumber(e);
        }

        private string GetLastNameFromFio(string fio)
        {
            var parts = fio.Trim().Split(' ');
            return parts.Length > 0 ? char.ToUpper(parts[0][0]) + parts[0].Substring(1).ToLower() : "";
        }

        private string GetNameFromFio(string fio)
        {
            var parts = fio.Trim().Split(' ');
            return parts.Length > 1 ? char.ToUpper(parts[1][0]) + parts[1].Substring(1).ToLower() : "";
        }

        private string GetPatronymicFromFio(string fio)
        {
            var parts = fio.Trim().Split(' ');
            return parts.Length > 2 ? char.ToUpper(parts[2][0]) + parts[2].Substring(1).ToLower() : "";
        }

        private bool Restrictions(string fio)
        {
            var parts = SplitFio(fio);

            if (parts.Length < 2 || parts.Length > 3)
            {
                MessageBox.Show("ФИО должно состоять из Фамилии, Имени и возможно Отчества", "Уведомление");
                return false;
            }

            if (!EditEmployeeWindow.IsValidFI(parts[0]))
            {
                MessageBox.Show("Фамилия должна содержать только кириллицу и дефис", "Уведомление");
                return false;
            }

            if (!EditEmployeeWindow.IsValidFI(parts[1]))
            {
                MessageBox.Show("Имя должно содержать только кириллицу и дефис", "Уведомление");
                return false;
            }

            if (parts.Length == 3 && !EditEmployeeWindow.IsValidPatr(parts[2]))
            {
                MessageBox.Show("Отчество должно содержать только кириллицу и дефис", "Уведомление");
                return false;
            }

            if(!EditEmployeeWindow.IsValidEmail(emailBox.Text))
            {   
                MessageBox.Show("Неверный формат электронной почты", "Уведомление");
                return false;
            }

            if (!EditEmployeeWindow.IsValidBirthDate(datebirth))
            {
                MessageBox.Show("Клиент должен быть старше 18 лет", "Уведомление");
                return false;
            }

            if(passwordBox.Password.Length < 5 || passwordBox.Password.Length > 25)
            {
                MessageBox.Show("Длина пароля не корректна", "Уведомление");
                return false;
            }
            if(passportBox.Text.Length < 10 && passportBox.Text.Length > 10)
            {
                MessageBox.Show("Не верная длина серии и номера паспорта", "Уведомление");
                return false;
            }

            return true;
        }
    }
}
