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
    /// Логика взаимодействия для BookEditWindow.xaml
    /// </summary>
    public partial class BookEditAddWindow : Window
    {
        private int editedBookId;

        public BookEditAddWindow(int editedBookId = 0)
        {
            this.editedBookId = editedBookId;

            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            try
            {
                BookshopEntities dataBase = new BookshopEntities();

                // Получение выбранной книги
                books selectedBook = dataBase.books.Where(b => b.id == this.editedBookId).FirstOrDefault();

                // Получение списка всех id книг
                List<int> id = (from b in dataBase.books
                                select b.id).ToList();

                // Получение списка всех названий книг
                List<string> titles = (from b in dataBase.books
                                       select b.book_name).ToList();

                // Получение списка всех ФИО авторов
                List<string> authors = (from b in dataBase.books
                                        select b.authors.last_names.last_name + " " + b.authors.first_names.first_name + " " + b.authors.middle_names.middle_name).ToList();

                // Получение списка всех названий изданий
                List<string> publishers = (from b in dataBase.books
                                           select b.publishers.publisher_name).ToList();

                // Получение списка всех жанров
                List<string> genres = (from b in dataBase.books
                                       select b.genres.genre_name).ToList();

                // Добавление в ComboBox данных о книгах
                this.titlesComboBox.ItemsSource = titles;
                this.authorsComboBox.ItemsSource = authors;
                this.publishersComboBox.ItemsSource = publishers;
                this.genresComboBox.ItemsSource = genres;

                // Заполняем ComboBox'ы данными о выбранной книге
                this.titlesComboBox.SelectedItem = titles[id.IndexOf(this.editedBookId)];
                this.authorsComboBox.SelectedItem = authors[id.IndexOf(this.editedBookId)];
                this.publishersComboBox.SelectedItem = publishers[id.IndexOf(this.editedBookId)];
                this.genresComboBox.SelectedItem = genres[id.IndexOf(this.editedBookId)];

                this.priceTextBox.Text = selectedBook.price.ToString();
                this.costTextBox.Text = selectedBook.cost.ToString();
                this.discountTextBox.Text = selectedBook.discount_percent.ToString();
                this.quantityTextBox.Text = selectedBook.quantity.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = false;
            }
        }

        public void SaveBook()
        {
            try
            {
                BookshopEntities dataBase = new BookshopEntities();

                books book;
                if (this.editedBookId == 0)
                {
                    book = new books();
                }
                else
                {
                    book = dataBase.books.Where(b => b.id == this.editedBookId).FirstOrDefault();
                }

                book.book_name = this.titlesComboBox.Text == book.book_name ? book.book_name : this.titlesComboBox.Text;

                book.authors = dataBase.authors.Where(a => a.last_names.last_name + " " + a.first_names.first_name + " " + a.middle_names.middle_name == this.authorsComboBox.Text).FirstOrDefault();
                book.publishers = dataBase.publishers.Where(p => p.publisher_name == this.publishersComboBox.Text).FirstOrDefault();
                book.genres = dataBase.genres.Where(p => p.genre_name == this.genresComboBox.Text).FirstOrDefault();

                book.price = int.Parse(this.priceTextBox.Text);
                book.cost = int.Parse(this.costTextBox.Text);

                book.discount_percent = int.Parse(this.discountTextBox.Text);

                book.quantity = int.Parse(this.quantityTextBox.Text);

                dataBase.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.titlesComboBox.Text) && !String.IsNullOrEmpty(this.authorsComboBox.Text) && !String.IsNullOrEmpty(this.publishersComboBox.Text) && !String.IsNullOrEmpty(this.genresComboBox.Text))
            {
                if (int.TryParse(this.priceTextBox.Text, out int price) && int.TryParse(this.costTextBox.Text, out int cost) && int.TryParse(this.discountTextBox.Text, out int discount) && int.TryParse(this.quantityTextBox.Text, out int quantity))
                {
                    if (price >= 0 && cost >= 0 && discount >= 0 && quantity >= 0)
                    {
                        this.DialogResult = true;
                    }
                }
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
