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
    /// <summary>
    /// Логика взаимодействия для PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        private ulong total;

        public PaymentWindow(ulong total)
        {
            InitializeComponent();

            this.total = total;
            this.cashTextBox.Text = this.total.ToString();
        }

        public ulong GetChange()
        {
            return ulong.Parse(this.cashTextBox.Text) - this.total;
        }

        private void numButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                this.cashTextBox.Text += button.Content;
            }
        }

        private void plusOneButton_Click(object sender, RoutedEventArgs e)
        {
            if (ulong.TryParse(this.cashTextBox.Text, out ulong cash))
            {
                this.cashTextBox.Text = (cash + 1).ToString();
            }
            else
            {
                this.cashTextBox.Text = this.total.ToString();
            }
        }

        private void minusOneButton_Click(object sender, RoutedEventArgs e)
        {
            if (ulong.TryParse(this.cashTextBox.Text, out ulong cash))
            {
                if (cash == 0)
                {
                    return;
                }
                this.cashTextBox.Text = (cash - 1).ToString();
            }
            else
            {
                this.cashTextBox.Text = this.total.ToString();
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.cashTextBox.Text.Length > 0)
            {
                this.cashTextBox.Text = this.cashTextBox.Text.Remove(this.cashTextBox.Text.Length - 1);
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            this.cashTextBox.Clear();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (ulong.TryParse(this.cashTextBox.Text, out ulong cash)) // проверка на то, что введённо в поле - число
            {
                if (cash >= this.total) // проверка, что введённое число больше или равно сумме
                {
                    this.DialogResult = true;
                }
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
