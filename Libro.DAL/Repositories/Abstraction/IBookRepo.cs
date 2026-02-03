namespace Libro.DAL.Repositories.Abstraction
{
    public interface IBookRepo : IRepository<Book>
    {
        Task<Book?> UpdateAsync(Book book, List<int?> categoryIds);
        Task<Book?> GetByIdWithCategoriesAsync(int id);
        Task<Book?> GetByIdWithAuthorAndCategoriesAsync(int id);
        Task<IEnumerable<Book>> GetByAuthorIdAsync(int authorId);
    }
}
//Task<Book?> UpdateAsync(Book book);
//Task<Book?> GetBookWithCategoriesAsync(int id);
//Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync();
//Task<bool> ExistsByTitleAndAuthorAsync(string title, int authorId, int? excludeId = null);


//Task<Book?> GetBookWithDetailsAsync(int id);
//Task<List<Book>> GetAllBooksWithDetailsAsync(Expression<Func<Book, bool>>? filter = null);
//Task<bool> IsBookTitleUniqueAsync(string title, int authorId, int? excludeId = null);