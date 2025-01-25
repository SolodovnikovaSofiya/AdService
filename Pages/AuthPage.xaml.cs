using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdService.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxLogin.Text;
            string password = PasswordBox.Password;

            if ( string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Введите логин");
                return;
            }
            else if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль");
                return;
            }

            using (var db = new Entities())
            {    
                var users = db.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.UserLogin == login && u.UserPassword == password);
                if (users == null)
                {
                    MessageBox.Show("Пользователь с такими данными не найден!");
                    return;
                }
                else
                {
                    MessageBox.Show("Авторизация успешна!");
                    NavigationService?.Navigate(new UserPage(users));
                }
            }
        }

        private void Btn_Back(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
