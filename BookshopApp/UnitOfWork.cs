using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookshopApp
{
    public class UnitOfWork
    {
        private BookshopEntities dataBase;
        private IEnumerable<object> books;

        public UnitOfWork()
        {
            this.dataBase = new BookshopEntities();

            this.books = (from b
                            in dataBase.books
                            select new
                            {
                                Id = b.id,
                                Title = b.book_name,
                                Author = b.authors.last_names.last_name + " " + b.authors.first_names.first_name + " " + b.authors.middle_names.middle_name,
                                Publisher = b.publishers.publisher_name,
                                Genre = b.genres.genre_name,
                                Quantity = b.quantity,
                                Price = b.price
                            }).ToList();
        }

        public IEnumerable<object> GetBooks()
        {
            return this.books;
        }

        public void UpdateBookQuantity(int id, string Authors)
        {

        }
    }
}
