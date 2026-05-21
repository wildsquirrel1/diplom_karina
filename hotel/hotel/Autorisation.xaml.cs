using hotel.Data;
using hotel.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hotel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Autorisation : Window
    {
        private string Password
        {
            get
            {
                if (passwordTB.Visibility == Visibility.Visible)
                    return passwordTB.Text;
                else
                    return passwordPB.Password;
            }
        }

        private bool isUpdateingPassPB = false;
        private bool isUpdateingPassTB = false;

        public Autorisation()
        {

            try
            {
                InitializeComponent();
            }
            catch (Exception)
            {

                MessageBox.Show("Попробуйте подключить API", "Уведомление");
                throw;
            }
        }

        private async void entryB_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(emailTB.Text) && string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Поля не могут быть пустыми!", "Уведомление");
                return;
            }
            else if (string.IsNullOrWhiteSpace(emailTB.Text))
            {
                MessageBox.Show("Поле с электронной почтой не может быть пустым!", "Уведомление");
                return;
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Поле с паролем не может быть пустым!", "Уведомление");
                return;
            }

            var result = await Api.Auth(email: emailTB.Text, password: Password);
            if (result == null)
            {
                MessageBox.Show("Сотрудник не найден", "Уведомление");
                return;
            }
            if (result.Status == 1)
            {
                MessageBox.Show("Вы были уволены!", "Уведомление");
                return;
            }
            if(result.IdhotelNavigation.Idhotel == null || result.IdhotelNavigation.Idhotel == 0)
            {
                MessageBox.Show("Вы еще не утверждены в отель.", "Уведомление");
                return;
            }
            switch (result.Idrole)
            {
                case 1:
                    MessageBox.Show($"Вы вошли как главный менеджер по отелям {result.Name} {result.Lastname}", "Уведомление");
                    HeadHotelManagerWindow headHotelManagerWindow = new HeadHotelManagerWindow(result);
                    this.Hide();
                    headHotelManagerWindow.Show();
                    ResetParams();
                    break;
                case 2:
                    MessageBox.Show($"Вы вошли как главный менеджер {result.Name} {result.Lastname}", "Уведомление");
                    MainManager mainManager = new MainManager(result);
                    this.Hide();
                    mainManager.Show();
                    ResetParams();
                    break;
                case 3:
                    MessageBox.Show($"Вы вошли как специалист службы бронирования {result.Name} {result.Lastname}", "Уведомление");
                    BookingServiceSpecialist bookingServiceSpecialist = new BookingServiceSpecialist(result);
                    this.Hide();
                    bookingServiceSpecialist.Show();
                    ResetParams();
                    break;
            }
        }

        private void ResetParams()
        {
            emailTB.Text = string.Empty;
            isUpdateingPassTB = true;
            passwordTB.Text = string.Empty;
            isUpdateingPassTB = false;
            isUpdateingPassPB = true;
            passwordPB.Password = string.Empty;
            isUpdateingPassPB = false;
            passwordTB.Visibility = Visibility.Collapsed;
            passwordPB.Visibility = Visibility.Visible;
            show_hide_pass.Content = "Показать пароль";
        }

        private void passwordPB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (isUpdateingPassTB)
                return;

            isUpdateingPassPB = true;
            passwordTB.Text = passwordPB.Password;
            isUpdateingPassPB = false;
        }

        private void passwordTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdateingPassPB)
                return;

            isUpdateingPassTB = true;
            passwordPB.Password = passwordTB.Text;
            isUpdateingPassTB = false;
        }

        private void show_hide_pass_Click(object sender, RoutedEventArgs e)
        {
            if (passwordPB.Visibility == Visibility.Visible)
            {
                isUpdateingPassTB = true;
                passwordTB.Text = passwordPB.Password;
                isUpdateingPassTB = false;
                passwordPB.Visibility = Visibility.Collapsed;
                passwordTB.Visibility = Visibility.Visible;
                passwordTB.Focus();
                passwordTB.CaretIndex = passwordTB.Text.Length;
                show_hide_pass.Content = "Скрыть пароль";
            }
            else
            {
                isUpdateingPassPB = true;
                passwordPB.Password = passwordTB.Text;
                isUpdateingPassPB = false;
                passwordTB.Visibility = Visibility.Collapsed;
                passwordPB.Visibility = Visibility.Visible;
                passwordPB.Focus();
                show_hide_pass.Content = "Показать пароль";
            }
        }
    }
}