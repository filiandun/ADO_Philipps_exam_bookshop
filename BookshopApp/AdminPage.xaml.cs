using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookshopApp
{
    public partial class AdminPage : Page
    {
        private byte currentGrid;

        public AdminPage()
        {
            this.currentGrid = 0;

            this.InitializeComponent();

            this.InitializeCatalog();
            this.InitializeArchive();

            this.InitializeAuthors();
            this.InitializePublishers();
            this.InitializeGenres();
        }

        private void InitializeCatalog()
        {
            try 
            {
                BookshopEntities dataBase = new BookshopEntities();

                var books = from b
                            in dataBase.books
                            select new
                            {
                                Id = b.id,
                                Title = b.book_name,

                                Author = b.authors.last_names.last_name + " " + b.authors.first_names.first_name + " " + b.authors.middle_names.middle_name,
                                Publisher = b.publishers.publisher_name,
                                Genre = b.genres.genre_name,

                                Price = b.price,
                                Cost = b.cost,

                                Discount = b.discount_percent,

                                Quantity = b.quantity
                            };

                this.catalogDataGrid.ItemsSource = books.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void InitializeArchive()
        {
            try 
            {
                BookshopEntities dataBase = new BookshopEntities();

                var books = from b
                            in dataBase.archive_books
                            select new
                            {
                                Id = b.id,
                                Title = b.book_name,

                                Author = b.authors.last_names.last_name + " " + b.authors.first_names.first_name + " " + b.authors.middle_names.middle_name,
                                Publisher = b.publishers.publisher_name,
                                Genre = b.genres.genre_name,

                                Price = b.price,
                                Cost = b.cost,

                                Discount = b.discount_percent,

                                Quantity = b.quantity
                            };

                this.archiveDataGrid.ItemsSource = books.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitializeAuthors()
        {
            try 
            {
                BookshopEntities dataBase = new BookshopEntities();

                var authors = from a in dataBase.authors
                              select new
                              {
                                  Id = a.id,
                                  Lastname = a.last_names.last_name,
                                  Firstname = a.first_names.first_name,
                                  Middlename = a.middle_names.middle_name,
                                  BooksQuantity = (from b in dataBase.books where b.author_id == a.id select b).Count()
                              };

                this.authorsDataGrid.ItemsSource = authors.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void InitializePublishers()
        {
            try
            {
                BookshopEntities dataBase = new BookshopEntities();

                var publishers = from p
                                 in dataBase.publishers
                                 select new
                                 {
                                    Id = p.id,

                                    Name = p.publisher_name,
                                    Address = p.publisher_address,

                                    BooksQuantity = (from b in dataBase.books where b.publisher_id == p.id select b).Count()
                                 };

                this.publishersDataGrid.ItemsSource = publishers.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void InitializeGenres()
        {
            try 
            {
                BookshopEntities dataBase = new BookshopEntities();

                var genres = from g
                             in dataBase.genres
                             select new
                             {
                                Id = g.id,

                                Name = g.genre_name,

                                BooksQuantity = (from b in dataBase.books where b.publisher_id == g.id select b).Count()
                             };

                this.genresDataGrid.ItemsSource = genres.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentGrid > 0)
            {
                --this.currentGrid;

                switch (this.currentGrid)
                {
                    case 0: this.gridTextBlock.Text = "Архив книг"; this.archiveDataGrid.Visibility = Visibility.Visible; this.archiveButtonUniformGrid.Visibility = Visibility.Visible; this.genresDataGrid.Visibility = Visibility.Hidden; this.apgButtonUniformGrid.Visibility = Visibility.Hidden; break;
                    case 1: this.gridTextBlock.Text = "Список авторов"; this.authorsDataGrid.Visibility = Visibility.Visible; this.apgButtonUniformGrid.Visibility = Visibility.Visible; this.archiveDataGrid.Visibility = Visibility.Hidden; this.archiveButtonUniformGrid.Visibility = Visibility.Hidden; break;
                    case 2: this.gridTextBlock.Text = "Список изданий"; this.publishersDataGrid.Visibility = Visibility.Visible; this.authorsDataGrid.Visibility = Visibility.Hidden; break;
                    case 3: this.gridTextBlock.Text = "Список жанров"; this.genresDataGrid.Visibility = Visibility.Visible; this.publishersDataGrid.Visibility = Visibility.Hidden; break;
                }
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentGrid < 3)
            {
                ++this.currentGrid;

                switch (this.currentGrid)
                {
                    case 0: this.gridTextBlock.Text = "Архив книг"; this.archiveDataGrid.Visibility = Visibility.Visible; this.archiveButtonUniformGrid.Visibility = Visibility.Visible; this.genresDataGrid.Visibility = Visibility.Hidden; this.apgButtonUniformGrid.Visibility = Visibility.Hidden; break;
                    case 1: this.gridTextBlock.Text = "Список авторов"; this.authorsDataGrid.Visibility = Visibility.Visible; this.apgButtonUniformGrid.Visibility = Visibility.Visible; this.archiveDataGrid.Visibility = Visibility.Hidden; this.archiveButtonUniformGrid.Visibility = Visibility.Hidden; break;
                    case 2: this.gridTextBlock.Text = "Список изданий"; this.publishersDataGrid.Visibility = Visibility.Visible; this.authorsDataGrid.Visibility = Visibility.Hidden; break;
                    case 3: this.gridTextBlock.Text = "Список жанров"; this.genresDataGrid.Visibility = Visibility.Visible; this.publishersDataGrid.Visibility = Visibility.Hidden; break;
                }
            }
        }

        private void restoreBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.archiveDataGrid.SelectedItem != null)
            {
                try
                {
                    BookshopEntities dataBase = new BookshopEntities();

                    int selectedBookId = (this.archiveDataGrid.SelectedItem as dynamic).Id;
                    archive_books selectedBook = dataBase.archive_books.Where(b => b.id == selectedBookId).FirstOrDefault();

                    books restoredBook = new books
                    {
                        id = selectedBook.id,
                        book_name = selectedBook.book_name,

                        author_id = selectedBook.author_id,
                        publisher_id = selectedBook.publisher_id,
                        genre_id = selectedBook.genre_id,

                        price = selectedBook.price,
                        cost = selectedBook.cost,

                        discount_percent = selectedBook.discount_percent,

                        quantity = selectedBook.quantity
                    };

                    dataBase.books.Add(restoredBook);
                    dataBase.archive_books.Remove(selectedBook);

                    dataBase.SaveChanges();

                    this.InitializeCatalog();
                    this.InitializeArchive();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void deleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.archiveDataGrid.SelectedItem != null)
            {
                MessageBoxResult firstResult = MessageBox.Show("Вы уверены, что хотите удалить книгу из архива?\nВозможности восстановить её не будет!", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (firstResult == MessageBoxResult.Yes)
                {
                    MessageBoxResult secondResult = MessageBox.Show("Вы точно уверены?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (secondResult == MessageBoxResult.Yes)
                    {
                        try
                        {
                            BookshopEntities dataBase = new BookshopEntities();

                            int selectedBookId = (this.archiveDataGrid.SelectedItem as dynamic).Id;
                            archive_books selectedBook = dataBase.archive_books.Where(b => b.id == selectedBookId).FirstOrDefault();

                            dataBase.archive_books.Remove(selectedBook);
                            dataBase.SaveChanges();

                            this.InitializeCatalog();
                            this.InitializeArchive();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        // КНОПКИ ДЛЯ УПРАВЛЕНИЯ АВТОРАМИ, ИЗДАТЕЛЯМИ И ЖАНРАМИ
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            switch (this.currentGrid)
            {
                case 1: break;
                case 2: break;
                case 3: break;
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            switch (this.currentGrid)
            {
                case 1: break;
                case 2: break;
                case 3: break;
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            switch (this.currentGrid)
            {
                case 1: break;
                case 2: break;
                case 3: break;
            }
        }

        private void filterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создание окна для выбора фильтров
                FilterWindow filterWindow = new FilterWindow();

                // Если была нажата кнопка "Подтвердить", то фильтрованный список книг выводится, иначе ничего не происходит
                if (filterWindow.ShowDialog() == true)
                {
                    this.catalogDataGrid.ItemsSource = filterWindow.GetFilteredCatalog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            // Инициализация каталога по новой, например, для сброса фильтров
            this.InitializeCatalog();
        }

        private void editBookButton_Click(object sender, RoutedEventArgs e) // TO DO
        {
            if (this.catalogDataGrid.SelectedItem != null)
            {
                try
                {
                    int selectedBookId = (this.catalogDataGrid.SelectedItem as dynamic).Id;

                    BookEditAddWindow bookEditWindow = new BookEditAddWindow(selectedBookId);

                    if (bookEditWindow.ShowDialog() == true)
                    {
                        bookEditWindow.SaveBook();

                        this.lastEditTextBlock.Text = $"[{selectedBookId}]";

                        this.InitializeCatalog();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void archiveBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.catalogDataGrid.SelectedItem != null)
            {
                MessageBox.Show("1");

                try
                {
                    BookshopEntities dataBase = new BookshopEntities();

                    int selectedBookId = (this.catalogDataGrid.SelectedItem as dynamic).Id;
                    books selectedBook = dataBase.books.Where(b => b.id == selectedBookId).FirstOrDefault();

                    archive_books archiveBook = new archive_books
                    { 
                        id = selectedBook.id,
                        book_name = selectedBook.book_name,

                        author_id = selectedBook.author_id,
                        publisher_id = selectedBook.publisher_id,
                        genre_id = selectedBook.genre_id,

                        price = selectedBook.price,
                        cost = selectedBook.cost,

                        discount_percent = selectedBook.discount_percent,

                        quantity = selectedBook.quantity
                    };

                    this.lastArchiveTextBlock.Text = $"[{selectedBook.id}] \"{selectedBook.book_name}\" {selectedBook.authors.last_names.last_name + " " + selectedBook.authors.first_names.first_name + " " + selectedBook.authors.middle_names.middle_name}";

                    dataBase.archive_books.Add(archiveBook);
                    dataBase.books.Remove(selectedBook);

                    dataBase.SaveChanges();

                    this.InitializeCatalog();
                    this.InitializeArchive();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException.Message);
                }
            }
        }


        private void addBookButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BookEditAddWindow bookEditWindow = new BookEditAddWindow();

                if (bookEditWindow.ShowDialog() == true)
                {
                    bookEditWindow.SaveBook();

                    this.InitializeCatalog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void addDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PromotionWindow promotionWindow;

                if (this.catalogDataGrid.SelectedItem != null)
                {
                    int selectedBookId = (this.catalogDataGrid.SelectedItem as dynamic).Id;

                    promotionWindow = new PromotionWindow(selectedBookId);
                }
                else
                {
                    promotionWindow = new PromotionWindow();
                }

                if (promotionWindow.ShowDialog() == true)
                {
                    promotionWindow.SetPromotion();
                    this.InitializeCatalog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
