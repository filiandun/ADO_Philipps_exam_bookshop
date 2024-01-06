using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public partial class FilterWindow : Window
    {
        public FilterWindow()
        {
            this.InitializeComponent();
            this.InitializeData();
        }

        private void InitializeData()
        {
            try
            {      
                BookshopEntities dataBase = new BookshopEntities();

                // Получение списка всех названий книг
                List<string> titles = (from b in dataBase.books
                                       where b.quantity > 0
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

                this.titlesComboBox.ItemsSource = titles;
                this.authorsComboBox.ItemsSource = authors;
                this.publishersComboBox.ItemsSource = publishers;
                this.genresComboBox.ItemsSource = genres;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = false;
            }
        }

        public dynamic GetFilteredCatalog()
        {
            try
            {
                BookshopEntities dataBase = new BookshopEntities();

                var books = (from b
                            in dataBase.books
                            // Когда ничего в comboBox не выбрано или в поле не введенно, то фильтр просто игнорируется
                            where (string.IsNullOrEmpty(this.titlesComboBox.Text) || b.book_name.Contains(this.titlesComboBox.Text)) &&
                                  (string.IsNullOrEmpty(this.authorsComboBox.Text) || (b.authors.last_names.last_name + " " + b.authors.first_names.first_name + " " + b.authors.middle_names.middle_name).Contains(this.authorsComboBox.Text)) &&
                                  (string.IsNullOrEmpty(this.publishersComboBox.Text) || b.publishers.publisher_name.Contains(this.publishersComboBox.Text)) &&
                                  (string.IsNullOrEmpty(this.genresComboBox.Text) || b.genres.genre_name.Contains(this.genresComboBox.Text))
                            select new
                            {
                                Id = b.id,
                                Title = b.book_name,

                                Author = b.authors.last_names.last_name + " " + b.authors.first_names.first_name + " " + b.authors.middle_names.middle_name,
                                Publisher = b.publishers.publisher_name,
                                Genre = b.genres.genre_name,

                                Quantity = b.quantity,

                                Price = b.price,
                                Cost = b.cost // ПРОВЕРИТЬ, МОЖЕТ ИСКЛЮЧЕНИЕ БУДЕТ, ТАК КАК В CatalogDataGrid У КАССИРА ЭТОГО СТОЛБЦА НЕТ, А У АДМИНА ЕСТЬ
                            }).ToList();

                return books;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Чтобы лишний раз не перерисовывать DataGrid, если ни один фильтр не введен
            if (String.IsNullOrEmpty(this.titlesComboBox.Text) && String.IsNullOrEmpty(this.authorsComboBox.Text) && String.IsNullOrEmpty(this.publishersComboBox.Text) && String.IsNullOrEmpty(this.genresComboBox.Text))
            {
                this.DialogResult = false;
                return; // обязателен, иначе исключение, так как иначе код продолжается и он пытается второй раз DialogResult назначить, который ниже
            }

            this.DialogResult = true;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
