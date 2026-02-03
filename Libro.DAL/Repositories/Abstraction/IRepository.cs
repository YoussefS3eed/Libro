namespace Libro.DAL.Repositories.Abstraction
{
    public interface IRepository<T> where T : class
    {
        // Command
        Task<T?> AddAsync(T entity);
        Task<bool> SaveChangesAsync();
        // Query
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}









//Task<T?> AddAsync(T entity);
//Task<T?> UpdateAsync(T entity);
//Task<bool> DeleteAsync(int id);
//Task<bool> SoftDeleteAsync(int id, string deletedBy);

//// Queries
//Task<T?> GetByIdAsync(int id);
//Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes);
//Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
//Task<List<T>> GetAllWithIncludesAsync(Expression<Func<T, bool>>? filter = null,
//    params Expression<Func<T, object>>[] includes);
//Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
//Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
//Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);