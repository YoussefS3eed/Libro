namespace Libro.DAL.Entities
{
    public class BookCategory
    {
        private BookCategory() { }
        public BookCategory(int bookId, int categoryId)
        {
            BookId = bookId;
            CategoryId = categoryId;
        }
        public int BookId { get; private set; }
        public Book? Book { get; private set; }
        public int? CategoryId { get; private set; }
        public Category? Category { get; private set; }
    }
}