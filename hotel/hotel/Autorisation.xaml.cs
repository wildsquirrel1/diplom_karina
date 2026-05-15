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
            if(!string.IsNullOrEmpty(passwordPB.Password))
            {
                passwordTB.Text = passwordPB.Password;
            }
            else if (!string.IsNullOrEmpty(passwordTB.Text))
            {
                passwordPB.Password = passwordTB.Text;
            }

            if (emailTB.Text == null && passwordTB.Text == null)
            {
                MessageBox.Show("Поля не могут быть пустыми!", "Уведомление");
                return;
            }
            else if(emailTB.Text == null)
            {
                MessageBox.Show("Поле с электронной почтой не может быть пустым!", "Уведомление");
                return;
            }
            else if (passwordTB.Text == null)
            {
                MessageBox.Show("Поле с паролем не может быть пустым!", "Уведомление");
                return;
            }

            var result = await Api.Auth(email: emailTB.Text, password: passwordTB.Text);
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
            passwordTB.Text = string.Empty;
            passwordPB.Password = string.Empty;
            
        }

        private void show_hide_pass_Click(object sender, RoutedEventArgs e)
        {
            if (passwordPB.Visibility == Visibility.Visible)
            {
                passwordTB.Text = passwordPB.Password;
                passwordTB.Visibility = Visibility.Visible;
                passwordPB.Visibility = Visibility.Hidden;
            }
            else
            {
                passwordPB.Password = passwordTB.Text;
                passwordPB.Visibility = Visibility.Visible;
                passwordTB.Visibility = Visibility.Hidden;
            }
        }
    }
}