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