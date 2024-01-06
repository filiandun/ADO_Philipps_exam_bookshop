using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace BookshopApp
{
    public partial class MainWindow : Window
    {
        private bool isAdmin;

        private AdminPage adminPage = new AdminPage();
        private CashierPage cashierPage = new CashierPage();

        public MainWindow()
        {
            InitializeTimer();
            InitializeComponent();

            LoginWindow loginWindow = new LoginWindow();

            if (loginWindow.ShowDialog() == false)
            {
                this.Close();
            }
            else if (loginWindow.CurrentUser.is_admin)
            {
                this.isAdmin = true;
            }
            else if (!loginWindow.CurrentUser.is_admin)
            {
                this.isAdmin = false;
            }

            this.MainFrame.Navigate(this.cashierPage);
        }

        private void InitializeTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.currentTimeLabel.Content = DateTime.Now.ToString();
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();

            if (loginWindow.ShowDialog() == false)
            {
                this.Close();
            }
            else if (loginWindow.CurrentUser.is_admin)
            {
                this.isAdmin = true;
            }
            else if (!loginWindow.CurrentUser.is_admin)
            {
                this.isAdmin = false;
            }

            this.MainFrame.RemoveBackEntry();
            this.MainFrame.Navigate(this.cashierPage);
            this.MainFrame.RemoveBackEntry();
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Если текущий пользователь - не администратор
            if (!this.isAdmin)
            {
                // Открывается окно авторизации для подтверждения администратора
                LoginWindow loginWindow = new LoginWindow("Необходим администратор");

                // !Если пользователь авторизовался и он не является администратором
                if (!(loginWindow.ShowDialog() == true && loginWindow.CurrentUser.is_admin))
                {
                    return;
                }
            }

            // Если пользователь это админ или все условия выши были выполнены
            this.isAdmin = true;
            this.MainFrame.Navigate(this.adminPage);
        }
    }
}
