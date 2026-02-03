namespace Libro.DAL.Repositories.Abstraction
{
    public interface ICategoryRepo : IRepository<Category>
    {
        // Command
        new Task<Category?> UpdateAsync(Category category);
        Task<Category?> ToggleStatusAsync(int id);
    }
}
