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
    /// Логика взаимодействия для PromotionWindow.xaml
    /// </summary>
    public partial class PromotionWindow : Window
    {
        public PromotionWindow(int selectedBookId = 0)
        {
            this.InitializeComponent();
            this.InitializeData(selectedBookId);
        }

        private void InitializeData(int selectedBookId = 0)
        {
            try
            {
                BookshopEntities dataBase = new BookshopEntities();

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

                // Чтобы пользователь мог выбрать пустую строку в фильтрах
                id.Insert(0, 0);
                titles.Insert(0, "");
                authors.Insert(0, "");
                publishers.Insert(0, "");
                genres.Insert(0, "");

                // Добавление в ComboBox данных о книгах
                this.titlesComboBox.ItemsSource = titles;
                this.authorsComboBox.ItemsSource = authors;
                this.publishersComboBox.ItemsSource = publishers;
                this.genresComboBox.ItemsSource = genres;

                // Если в каталоге была выбрана какая-то книга, то заполняем ComboBox'ы данными о ней
                if (selectedBookId != 0)
                {
                    this.titlesComboBox.SelectedItem = titles[id.IndexOf(selectedBookId)];
                    this.authorsComboBox.SelectedItem = authors[id.IndexOf(selectedBookId)];
                    this.publishersComboBox.SelectedItem = publishers[id.IndexOf(selectedBookId)];
                    this.genresComboBox.SelectedItem = genres[id.IndexOf(selectedBookId)];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = false;
            }
        }

        public void SetPromotion()
        {
            try
            {
                BookshopEntities dataBase = new BookshopEntities();

                // Получение всех книг по фильтрам
                IQueryable<books> books = from b in dataBase.books
                                          where ((string.IsNullOrEmpty(this.titlesComboBox.SelectedItem.ToString()) || b.book_name == this.titlesComboBox.SelectedItem.ToString()) &&
                                          (string.IsNullOrEmpty(this.authorsComboBox.SelectedItem.ToString()) || b.authors.last_names.last_name + " " + b.authors.first_names.first_name + " " + b.authors.middle_names.middle_name == this.authorsComboBox.SelectedItem.ToString()) &&
                                          (string.IsNullOrEmpty(this.publishersComboBox.SelectedItem.ToString()) || b.publishers.publisher_name == this.publishersComboBox.SelectedItem.ToString()) &&
                                          (string.IsNullOrEmpty(this.genresComboBox.SelectedItem.ToString()) || b.genres.genre_name == this.genresComboBox.SelectedItem.ToString()))
                                          select b;

                // Установка процента скидки у книг
                int discount = int.Parse(this.discountTextBox.Text);
                foreach (books book in books)
                {
                    book.discount_percent = discount;
                }

                dataBase.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Чтобы лишний раз не перерисовывать DataGrid, если ни один фильтр не введен
            if (string.IsNullOrEmpty(this.titlesComboBox.SelectedItem.ToString()) && string.IsNullOrEmpty(this.authorsComboBox.SelectedItem.ToString()) && string.IsNullOrEmpty(this.publishersComboBox.SelectedItem.ToString()) && string.IsNullOrEmpty(this.genresComboBox.SelectedItem.ToString()))
            {
                this.DialogResult = false;
                return; // обязателен, иначе исключение, так как иначе код продолжается и он пытается второй раз DialogResult назначить, который ниже
            }

            // Проверка на то, что введённый процент скидки не больше 100 и не меньше нуля
            if (int.TryParse(this.discountTextBox.Text, out int promotion))
            {
                MessageBox.Show(promotion.ToString());
                if (promotion >= 0 && promotion <= 100)
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
