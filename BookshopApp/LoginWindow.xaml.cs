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

namespace BookshopApp
{
    public partial class LoginWindow : Window
    {
        private SolidColorBrush blackBrush = new SolidColorBrush(Colors.Black);
        private SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Colors.Gray);

        private users currentUser;

        public LoginWindow()
        {
            InitializeComponent();
        }

        public LoginWindow(string label)
        {
            InitializeComponent();

            this.mainLabel.Content = label;
        }

        public users CurrentUser
        {
            get
            {
                return this.currentUser;
            }
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            string login = this.loginTextBox.Text;
            string password = this.passwordBox.Password;

            if (!String.IsNullOrEmpty(login) && !String.IsNullOrEmpty(password))
            {
                using (BookshopEntities dataBase = new BookshopEntities())
                {
                    users user = dataBase.users.Where(u => u.login == login).FirstOrDefault();

                    if (user != null)
                    {
                        if (user.password == password)
                        {
                            this.currentUser = user;

                            this.DialogResult = true;
                        }

                        this.passwordBox.Password = "";

                        this.passwordBox.BorderBrush = this.redBrush;
                        this.passwordLabel.Foreground = this.redBrush;

                        this.passwordLabel.Content = "Пароль неверный";
                        this.passwordLabel.Visibility = Visibility.Visible;

                        return;
                    }

                    this.passwordBox.Password = "";
                    this.loginTextBox.Text = "";

                    this.loginTextBox.BorderBrush = this.redBrush;
                    this.loginLabel.Foreground = this.redBrush;

                    this.loginLabel.Content = "Логин не найден";
                    this.loginLabel.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (String.IsNullOrEmpty(login))
                {
                    this.loginTextBox.BorderBrush = this.redBrush;
                    this.loginLabel.Foreground = this.redBrush;

                    this.loginLabel.Content = "Тут введите ваш логин";
                    this.loginLabel.Visibility = Visibility.Visible;
                }

                if (String.IsNullOrEmpty(password))
                {
                    this.passwordBox.BorderBrush = this.redBrush;
                    this.passwordLabel.Foreground = this.redBrush;

                    this.passwordLabel.Content = "Тут введите ваш пароль";
                    this.passwordLabel.Visibility = Visibility.Visible;
                }
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.loginTextBox.BorderBrush = this.grayBrush;
            this.loginLabel.Foreground = this.blackBrush;

            if (String.IsNullOrEmpty(this.loginTextBox.Text))
            {
                this.loginLabel.Visibility = Visibility.Visible;
            }
            else
            {
                this.loginLabel.Visibility = Visibility.Hidden;
            }
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.passwordBox.BorderBrush = this.grayBrush;
            this.passwordLabel.Foreground = this.blackBrush;

            if (String.IsNullOrEmpty(this.passwordBox.Password))
            {
                this.passwordLabel.Visibility = Visibility.Visible;
            }
            else
            {
                this.passwordLabel.Visibility = Visibility.Hidden;
            }
        }
    }
}
