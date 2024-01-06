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
    public partial class QuantityInputWindow : Window
    {
        private string bookInfo;

        private int maxQuantity; // количество книг в наличии
        private int inputQuantity;

        public QuantityInputWindow(string bookInfo, int maxQuantity)
        {
            InitializeComponent();

            this.bookInfo = bookInfo;
            this.maxQuantity = maxQuantity;
            this.inputQuantity = 1;

            this.bookInfoTextBlock.Text = bookInfo;
            this.quantityTextBox.Text = "1";
        }

        public int GetQuantity()
        {
            return this.inputQuantity;
        }

        void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void numButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                this.quantityTextBox.Text += button.Content;
            }
        }

        private void plusOneButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(this.quantityTextBox.Text, out int quantity))
            {
                this.quantityTextBox.Text = (quantity + 1).ToString();
            }
            else
            {
                this.quantityTextBox.Text = "1";
            }
        }

        private void minusOneButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(this.quantityTextBox.Text, out int quantity))
            {
                if (quantity == 0)
                {
                    return;
                }
                this.quantityTextBox.Text = (quantity - 1).ToString();
            }
            else
            {
                this.quantityTextBox.Text = "1";
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.quantityTextBox.Text.Length > 0)
            {
                this.quantityTextBox.Text = this.quantityTextBox.Text.Remove(this.quantityTextBox.Text.Length - 1);
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            this.quantityTextBox.Clear();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(this.quantityTextBox.Text, out int quantity))
            {
                if (quantity > 0)
                {
                    if (quantity > this.maxQuantity)
                    {
                        MessageBox.Show($"В наличие книг \"{this.bookInfoTextBlock.Text}\" есть только {this.maxQuantity}.");
                        return;
                    }

                    this.inputQuantity = quantity;
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
