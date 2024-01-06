using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BookshopApp
{
    public partial class CashierPage : Page
    {
        private bool isPaidByCard;

        public CashierPage()
        {
            this.InitializeComponent();
            this.InitializeCatalog();

            this.isPaidByCard = false;

            this.checkButton.IsEnabled = false;
            this.paymentGrid.Visibility = Visibility.Hidden;
        }

        private void InitializeCatalog()
        {
            using (BookshopEntities dataBase = new BookshopEntities())
            {
                var books = from b
                            in dataBase.books
                            where (b.quantity > 0)
                            select new
                            {
                                Id = b.id,
                                Title = b.book_name,

                                Author = b.authors.last_names.last_name + " " + b.authors.first_names.first_name + " " + b.authors.middle_names.middle_name,
                                Publisher = b.publishers.publisher_name,
                                Genre = b.genres.genre_name,

                                Price = b.price,
                                Discount = b.discount_percent,

                                Quantity = b.quantity,
                            };

                this.catalogDataGrid.ItemsSource = books.ToList();
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.catalogDataGrid.SelectedItem != null)
            {
                try
                {
                    foreach (var selectedItem in this.catalogDataGrid.SelectedItems)
                    {
                        // Получить выбранный элемент из первого DataGrid
                        var selectedBook = (dynamic)selectedItem;

                        // Отображение окно с выбором количества покупаемоей книги
                        int maxQuantity = selectedBook.Quantity;
                        string bookInfo = "\"" + selectedBook.Title + "\" " + selectedBook.Author + " - " + selectedBook.Publisher;
                        QuantityInputWindow quantityInputWindow = new QuantityInputWindow(bookInfo, maxQuantity);

                        if (quantityInputWindow.ShowDialog() == false)
                        {
                            return;
                        }

                        int quantity = quantityInputWindow.GetQuantity(); // получение количества

                        // Проверка на наличие книги в корзине
                        for (int i = 0; i < this.cartDataGrid.Items.Count; i++)
                        {
                            dynamic item = this.cartDataGrid.Items[i];

                            if (item.Id == selectedBook.Id) // если такая книга уже есть
                            {
                                quantity += item.Quantity; // прибавляется старое количество книг к новому
                                this.cartDataGrid.Items.RemoveAt(i); // книга удаляется из корзины (чтобы потом вставиться с новым количеством)

                                break;
                            }
                        }

                        // Создание новой книги, на основе выбранной книги
                        dynamic book = new
                        {
                            Id = selectedBook.Id,
                            Title = selectedBook.Title,

                            Author = selectedBook.Author,

                            Price = selectedBook.Price,
                            Discount = selectedBook.Discount,

                            Quantity = quantity,
                            Sum = (quantity * selectedBook.Price) - ((quantity * selectedBook.Price) * (selectedBook.Discount == 0 ? 1 : selectedBook.Discount / 100.0))
                        };

                        // Добавление новой книги в корзину
                        this.cartDataGrid.Items.Add(book);

                        // Сортировка
                        this.SortCart();

                        // Обновление totalTextBlock с итоговой ценой всей корзины
                        this.UpdateTotal();

                        // Обновление каталога, так как книги резервируются
                        this.UpdateBookQuantity(false, book.Id, book.Quantity);

                        /*
                         * dynamic - это тип данных в C#, который позволяет отложить проверку типов до времени выполнения программы. 
                         * В отличие от статических типов данных, таких как int или string, dynamic позволяет выполнять операции без явного указания типа. 
                         * Когда мы используем dynamic, компилятор C# не проверяет типы переменной во время компиляции, а вместо этого делегирует проверку типов до времени выполнения программы. 
                         * Это означает, что мы можем выполнять операции и получать доступ к свойствам и методам объекта, используя dynamic, даже если компилятор не знает точного типа объекта. 
                         * В приведенном коде, мы используем dynamic, чтобы получить доступ к свойствам выбранного элемента из первого DataGrid, таким как Id, Title, Author и Price. 
                         * Это позволяет нам создать новый объект, представляющий выбранный элемент, без явного указания его типа. 
                         * Однако, следует заметить, что использование dynamic может снизить производительность программы, так как проверка типов выполняется во время выполнения. 
                         * Поэтому рекомендуется использовать dynamic только тогда, когда это действительно необходимо, например, когда работа с объектами неизвестного типа. 
                         * В остальных случаях рекомендуется использовать статические типы данных.
                         */
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void UpdateTotal()
        {
            int sum = 0;

            foreach (dynamic book in this.cartDataGrid.Items)
            {
                sum += book.Sum;
            }

            this.totalTextBlock.Text = sum.ToString();
        }

        private void UpdateAllBooksQuantity(bool sign)
        {
            try
            {
                BookshopEntities dataBase = new BookshopEntities();

                foreach (dynamic book in this.cartDataGrid.Items)
                {
                    int bookId = book.Id;
                    int bookQuantity = book.Quantity;

                    dataBase.books.Where(b => b.id == bookId).FirstOrDefault().quantity += sign ? bookQuantity : -bookQuantity;
                }
                dataBase.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.InitializeCatalog();
        }

        private void UpdateBookQuantity(bool sign, int bookId, int bookQuantity)
        {
            try
            {
                BookshopEntities dataBase = new BookshopEntities();

                dataBase.books.Where(b => b.id == bookId).FirstOrDefault().quantity += sign ? bookQuantity : -bookQuantity;

                dataBase.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.InitializeCatalog();
        }

        private void SortCart()
        {
            this.cartDataGrid.Items.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
            this.cartDataGrid.Items.Refresh();
        }

        private void numButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.cartDataGrid.SelectedItem != null)
            {
                if (sender is Button numButton)
                {
                    try
                    {
                        // Получение индекса выделенного элемента,
                        // чтобы в дальнейшем создать иллюзию того,
                        // что книга с таблицы не удалялась, а просто изменилось кол-во
                        int oldSelectedItemIndex = this.cartDataGrid.SelectedIndex;

                        // Инициализация старой информации о книги
                        dynamic oldBook = this.cartDataGrid.SelectedItem;
                        int oldBookId = oldBook.Id;
                        int oldBookQuantity = oldBook.Quantity;

                        // Получение нового кол-ва
                        int newQuantity = int.Parse($"{oldBookQuantity.ToString() + numButton.Content}");
                        if (newQuantity == 0)
                        {
                            newQuantity = 1;
                        }

                        // Проверка на то, что кол-во книг в корзине не превысит кол-ва книг в каталоге
                        BookshopEntities dataBase = new BookshopEntities();
                        int bookQuantityInStock = oldBookQuantity + dataBase.books.Where(b => b.id == oldBookId).FirstOrDefault().quantity;
                        if (bookQuantityInStock - newQuantity < 0)
                        {
                            return;
                        }

                        // Инициализация новой информации о книги
                        dynamic newBook = new
                        {
                            Id = oldBookId,
                            Title = oldBook.Title,

                            Author = oldBook.Author,

                            Price = oldBook.Price,
                            Discount = oldBook.Discount,

                            Quantity = newQuantity,
                            Sum = (newQuantity * oldBook.Price) - ((newQuantity * oldBook.Price) * (oldBook.Discount == 0 ? 1 : oldBook.Discount / 100.0))
                        };

                        // Удаление старой информации о книги и добавление новой
                        this.cartDataGrid.Items.Remove(oldBook);
                        this.cartDataGrid.Items.Add(newBook);

                        // Сортировка и обновление итоговой стоимости корзины
                        this.SortCart();
                        this.UpdateTotal();

                        this.UpdateBookQuantity(false, newBook.Id, newBook.Quantity - oldBookQuantity);

                        // Описание в самом начале
                        this.cartDataGrid.SelectedIndex = oldSelectedItemIndex;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void plusOneButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.cartDataGrid.SelectedItem != null)
            {
                try
                {
                    // Получение индекса выделенного элемента,
                    // чтобы в дальнейшем создать иллюзию того,
                    // что книга с таблицы не удалялась, а просто изменилось кол-во
                    int oldSelectedItemIndex = this.cartDataGrid.SelectedIndex;

                    // Инициализация старой информации о книги
                    dynamic oldBook = this.cartDataGrid.SelectedItem;
                    int oldBookId = oldBook.Id;
                    int oldBookQuantity = oldBook.Quantity;

                    // Получение нового кол-ва
                    int newQuantity = oldBookQuantity + 1;

                    // Проверка на то, что кол-во книг в корзине не превысит кол-ва книг в каталоге
                    BookshopEntities dataBase = new BookshopEntities();
                    int bookQuantityInStock = oldBookQuantity + dataBase.books.Where(b => b.id == oldBookId).FirstOrDefault().quantity;
                    if (bookQuantityInStock - newQuantity < 0)
                    {
                        return;
                    }

                    // Инициализация новой информации о книги
                    dynamic newBook = new
                    {
                        Id = oldBookId,
                        Title = oldBook.Title,

                        Author = oldBook.Author,

                        Price = oldBook.Price,
                        Discount = oldBook.Discount,

                        Quantity = newQuantity,
                        Sum = (newQuantity * oldBook.Price) - ((newQuantity * oldBook.Price) * (oldBook.Discount == 0 ? 1 : oldBook.Discount / 100.0))
                    };

                    // Удаление старой информации о книги и добавление новой
                    this.cartDataGrid.Items.Remove(oldBook);
                    this.cartDataGrid.Items.Add(newBook);

                    // Сортировка и обновление итоговой стоимости корзины
                    this.SortCart();
                    this.UpdateTotal();

                    this.UpdateBookQuantity(false, newBook.Id, 1);

                    // Описание в самом начале
                    this.cartDataGrid.SelectedIndex = oldSelectedItemIndex;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void minusOneButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.cartDataGrid.SelectedItem != null)
            {
                try
                {
                    // Инициализация старой информации о книги
                    dynamic oldBook = this.cartDataGrid.SelectedItem;

                    if (oldBook.Quantity > 1)
                    {
                        // Получение индекса выделенного элемента,
                        // чтобы в дальнейшем создать иллюзию того,
                        // что книга с таблицы не удалялась, а просто изменилось кол-во
                        int oldSelectedItemIndex = this.cartDataGrid.SelectedIndex;

                        // Получение нового кол-ва
                        int newQuantity = oldBook.Quantity - 1;

                        // Инициализация новой информации о книги
                        dynamic newBook = new
                        {
                            Id = oldBook.Id,
                            Title = oldBook.Title,

                            Author = oldBook.Author,

                            Price = oldBook.Price,
                            Discount = oldBook.Discount,

                            Quantity = newQuantity,
                            Sum = (newQuantity * oldBook.Price) - ((newQuantity * oldBook.Price) * (oldBook.Discount == 0 ? 1 : oldBook.Discount / 100.0))
                        };

                        // Удаление старой информации о книги и добавление новой
                        this.cartDataGrid.Items.Remove(oldBook);
                        this.cartDataGrid.Items.Add(newBook);

                        // Сортировка и обновление итоговой стоимости корзины
                        this.SortCart();
                        this.UpdateTotal();

                        this.UpdateBookQuantity(true, newBook.Id, 1);

                        // Описание в самом начале
                        this.cartDataGrid.SelectedIndex = oldSelectedItemIndex;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void backspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.cartDataGrid.SelectedItem != null)
            {
                try
                {
                    // Получение индекса выделенного элемента,
                    // чтобы в дальнейшем создать иллюзию того,
                    // что книга с таблицы не удалялась, а просто изменилось кол-во
                    int oldSelectedItemIndex = this.cartDataGrid.SelectedIndex;

                    // Инициализация старой информации о книги
                    dynamic oldBook = this.cartDataGrid.SelectedItem;

                    // Получение нового кол-ва
                    int newQuantity = oldBook.Quantity / 10;
                    if (newQuantity == 0)
                    {
                        newQuantity = 1;
                    }

                    // Инициализация новой информации о книги
                    dynamic newBook = new
                    {
                        Id = oldBook.Id,
                        Title = oldBook.Title,

                        Author = oldBook.Author,

                        Price = oldBook.Price,
                        Discount = oldBook.Discount,

                        Quantity = newQuantity,
                        Sum = (newQuantity * oldBook.Price) - ((newQuantity * oldBook.Price) * (oldBook.Discount == 0 ? 1 : oldBook.Discount / 100.0))
                    };

                    // Удаление старой информации о книги и добавление новой
                    this.cartDataGrid.Items.Remove(oldBook);
                    this.cartDataGrid.Items.Add(newBook);

                    // Сортировка и обновление итоговой стоимости корзины
                    this.SortCart();
                    this.UpdateTotal();

                    this.UpdateBookQuantity(true, newBook.Id, oldBook.Quantity - newBook.Quantity);

                    // Описание в самом начале
                    this.cartDataGrid.SelectedIndex = oldSelectedItemIndex;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.cartDataGrid.SelectedItem != null)
            {
                try
                {
                    dynamic book = this.cartDataGrid.SelectedItem;
                    this.UpdateBookQuantity(true, book.Id, book.Quantity);

                    this.cartDataGrid.Items.Remove(this.cartDataGrid.SelectedItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
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

        private void payButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка наличия книг в корзине
            if (this.cartDataGrid.Items.Count != 0)
            {
                // Дальше просто видимость UI меняется
                this.catalogDataGrid.Visibility = Visibility.Hidden;
                this.paymentGrid.Visibility = Visibility.Visible;

                this.payButton.IsEnabled = false;

                this.addButton.IsEnabled = false;
                this.deleteButton.IsEnabled = false;

                foreach (Button button in this.numsUniformGrid.Children)
                {
                    button.IsEnabled = false;
                }

                foreach (Button button in this.enterBackUniformGrid.Children)
                {
                    button.IsEnabled = false;
                }

                foreach (Button button in this.controlsUniformGrid.Children)
                {
                    button.IsEnabled = false;
                }
            }
        }

        private void cashPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.cartDataGrid.Items.Count != 0)
            {
                try
                {
                    PaymentWindow paymentWindow = new PaymentWindow(ulong.Parse(this.totalTextBlock.Text));

                    if (paymentWindow.ShowDialog() == true)
                    {
                        this.changeLabel.Content = paymentWindow.GetChange().ToString();

                        this.checkButton.IsEnabled = true;

                        this.cashPaymentButton.IsEnabled = false;
                        this.cardPaymentButton.IsEnabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void cardPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Оплата по карте успешно принята", "Оплата по карте", MessageBoxButton.OK);

            this.checkButton.IsEnabled = true;

            this.cashPaymentButton.IsEnabled = false;
            this.cardPaymentButton.IsEnabled = false;

            this.isPaidByCard = true;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            // Если оплата была произведена
            if (this.isPaidByCard)
            {
                // Открывается окно авторизации для подтверждения администратора
                LoginWindow loginWindow = new LoginWindow("Необходим администратор");

                // !Если пользователь авторизовался и он является администратором
                if (!(loginWindow.ShowDialog() == true && loginWindow.CurrentUser.is_admin))
                {
                    return;
                }
            }

            // Если оплата по карте ещё не была произведена (!this.isPaidByCard) или все условия выши были выполнены
            this.SetUIVisibleAndEnabled();
        }

        private void checkButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this.GenerateCheck());

            this.SetUIVisibleAndEnabled();
            this.UpdateAllBooksQuantity(false);
            this.cartDataGrid.Items.Clear();
            this.UpdateTotal();
        }

        private void SetUIVisibleAndEnabled()
        {
            this.catalogDataGrid.Visibility = Visibility.Visible;
            this.paymentGrid.Visibility = Visibility.Hidden;

            this.payButton.IsEnabled = true;

            this.cashPaymentButton.IsEnabled = true;
            this.cardPaymentButton.IsEnabled = true;

            this.addButton.IsEnabled = true;
            this.deleteButton.IsEnabled = true;

            foreach (Button button in this.numsUniformGrid.Children)
            {
                button.IsEnabled = true;
            }

            foreach (Button button in this.enterBackUniformGrid.Children)
            {
                button.IsEnabled = true;
            }

            foreach (Button button in this.controlsUniformGrid.Children)
            {
                button.IsEnabled = true;
            }

            this.changeLabel.Content = "0";
            this.isPaidByCard = false;
        }

        private string GenerateCheck()
        {
            StringBuilder receipt = new StringBuilder();

            receipt.AppendLine("=========================================");
            receipt.AppendLine("\t\tКнижный магазин");
            receipt.AppendLine("=========================================");
            receipt.AppendLine($"Дата/Время:\t\t\t{DateTime.Now}");
            receipt.AppendLine("=========================================");
            receipt.AppendLine("Название товара\tЦена\t\tКол-во\t\tСумма");
            receipt.AppendLine("---------------------------------------------------------------------------------");

            // Взятие каждой отдельной книги из корзины и добавление её в чек
            foreach (dynamic book in this.cartDataGrid.Items)
            {
                receipt.AppendLine($"{book.Title}\t\t{book.Price}\t\t{book.Quantity}\t\t{book.Sum}");
            }

            receipt.AppendLine("---------------------------------------------------------------------------------");
            receipt.AppendLine($"Итого:\t\t\t\t\t\t{this.totalTextBlock.Text}");
            receipt.AppendLine("=========================================");

            return receipt.ToString();
        }
    }
}
