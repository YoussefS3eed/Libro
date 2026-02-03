namespace Libro.DAL.Repositories.Abstraction
{
    public interface IAuthorRepo : IRepository<Author>
    {
        // Command
        Task<Author?> UpdateAsync(Author author);
        Task<Author?> ToggleStatusAsync(int id);
    }
}
