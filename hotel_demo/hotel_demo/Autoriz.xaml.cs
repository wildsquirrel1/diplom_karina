using hotel_demo.Model;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xaml;

namespace hotel_demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Autoriz : Window
    {
        private string Password
        {
            get
            {
                if(passwordTB.Visibility == Visibility.Visible)
                    return passwordTB.Text;
                else
                    return passwordPB.Password;
            }
        }

        private bool isUpdatingPassPB = false;
        private bool isUpdatingPassTB = false;
        public Autoriz()
        {
            InitializeComponent();

            passwordPB.PasswordChanged += passwordPB_PasswordChanged;
            passwordTB.TextChanged += passwordTB_TextChanged;

        }
        int count;
        public static Employeer CheckLoginPass (string email, string password, string pass)
        {
            var employee = App.contex.Employeers.FirstOrDefault(u => (u.Email == email && u.Password == password) 
            && (u.Email == email && u.Password == pass));
            if (employee == null)
            {
                MessageBox.Show("Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные", "Ошибка");
                

                return null;
            }
            return employee;
        }

        private void voity_Click(object sender, RoutedEventArgs e)
        {
            if(passwordTB.Text != "")
            {
                passwordPB.Password = passwordTB.Text;
            }
            else
            {
                passwordTB.Text = passwordPB.Password;
            }
            if((passwordTB.Text == "" && emailTB.Text == "") || (passwordPB.Password == null && emailTB.Text == ""))
            {
                MessageBox.Show("Оба поля должны быть заполнены!", "Ошибка");
                return;
            }
            else if (emailTB.Text == "")
            {
                MessageBox.Show("Поле логина должно быть заполнено", "Ошибка");
                return;
            }
            else if (passwordPB.Password == null || passwordTB.Text == "")
            {
                MessageBox.Show("Поле пароля должно быть заполнено", "Ошибка");
                return;
            }

            var emp = CheckLoginPass(emailTB.Text, passwordTB.Text, passwordPB.Password);
            
            if (count >= 3)
            {
                MessageBox.Show("Вы заблокированы. Обратитесь к администратору", "Ошибка");
                emp = App.contex.Employeers.FirstOrDefault(x => x.Email == emailTB.Text);
                emp.Isblocked = true;   
                App.contex.SaveChanges();
            }
            else if(emp == null)
            {
                count++;
                return;
            }
            else if (emp.Isblocked == true)
            {
                MessageBox.Show("Не возможно выполнить вход! Обратитесь к администратору!", "Ошибка");
                return;
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            DateOnly oneMonthAgo = today.AddDays(-30);
            if(emp.LastEntry <= oneMonthAgo)
            {
                MessageBox.Show("Вы долго не заходили в аккаунт, вы заблокированы", "Ошибка");
                emp.Isblocked = true;
                App.contex.SaveChanges();
                return;
            }
            else
            {
                switch (emp.Roleid)
                {
                    case 1:
                        
                        MessageBox.Show("Вы успешно авторизовались","Успешно");
                        emp.LastEntry = today;
                        ResetParams();
                        this.Hide();
                        AdminWindow admin = new AdminWindow();
                        emp.Entrycount++;
                        admin.Show();
                        break;
                    case 2:
                        if (count > 3)
                        {
                            MessageBox.Show("Вы заблокированы. Обратитесь к администратору", "Ошибка");
                            emp.Isblocked = true;
                            App.contex.SaveChanges();
                            break;
                        }
                        if(emp.Entrycount < 1)
                        {
                            EmployeeWindow employeeWindow = new EmployeeWindow(emp);
                            MessageBox.Show("Вы успешно авторизовались", "Успешно");
                            employeeWindow.Show();
                            emp.Entrycount++;

                            //this.Hide();
                            ResetParams();
                            emp.LastEntry = today;
                            break;
                        }
                        else
                        {
                            EmptyWindowForEmployee forEmployee = new EmptyWindowForEmployee();
                            MessageBox.Show("Вы успешно авторизовались", "Успешно");
                            emp.Entrycount++;
                            forEmployee.ShowDialog();
                            this.Hide();
                            ResetParams();
                            emp.LastEntry = today;
                            break;

                        }
                        
                }
            }
            
        }

        private void ResetParams()
        {
            emailTB.Text = "";
            passwordTB.Text = "";
            passwordPB.Password = null;
        }

        private void passwordTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingPassPB) return;

            isUpdatingPassTB = true;
            passwordPB.Password = passwordTB.Text;
            isUpdatingPassTB = false;
        }

        private void passwordPB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(isUpdatingPassTB) return;

            isUpdatingPassPB = true;
            passwordTB.Text = passwordPB.Password;
            isUpdatingPassPB = false;
        }
    }
}