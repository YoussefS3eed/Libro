namespace Libro.DAL.Repositories.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly LibroDbContext _context;
        public Repository(LibroDbContext context)
        {
            _context = context;
        }
        public async Task<bool> SaveChangesAsync()
            => await _context.SaveChangesAsync() > 0;
        public async Task<T?> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            if (await SaveChangesAsync())
                return entity;
            return null;
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await _context.Set<T>().AnyAsync(predicate);
        public async Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
            await _context.Set<T>().SingleOrDefaultAsync(predicate);
        public async Task<T?> GetByIdAsync(int id)
            => await _context.Set<T>().FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }
    }
}












//protected readonly LibroDbContext _context;
//protected readonly DbSet<T> _dbSet;

//public Repository(LibroDbContext context)
//{
//    _context = context;
//    _dbSet = context.Set<T>();
//}

//public virtual async Task<T?> AddAsync(T entity)
//{
//    await _dbSet.AddAsync(entity);
//    return await SaveChangesAsync() ? entity : null;
//}

//public virtual async Task<T?> UpdateAsync(T entity)
//{
//    var x = GetByIdAsync((int)typeof(T).GetProperty("Id")!.GetValue(entity)!);
//    _dbSet.Update(entity);
//    return await SaveChangesAsync() ? entity : null;
//}

//public virtual async Task<bool> DeleteAsync(int id)
//{
//    var entity = await GetByIdAsync(id);
//    if (entity == null) return false;

//    _dbSet.Remove(entity);
//    return await SaveChangesAsync();
//}

//public virtual async Task<bool> SoftDeleteAsync(int id, string deletedBy)
//{
//    var entity = await GetByIdAsync(id);
//    if (entity is BaseEntity baseEntity)
//    {
//        baseEntity.IsDeleted = true;
//        baseEntity.DeletedBy = deletedBy;
//        baseEntity.DeletedOn = DateTime.UtcNow;
//        return await SaveChangesAsync();
//    }
//    return false;
//}

//public virtual async Task<T?> GetByIdAsync(int id)
//    => await _dbSet.FindAsync(id);

//public virtual async Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes)
//{
//    IQueryable<T> query = _dbSet;

//    foreach (var include in includes)
//    {
//        query = query.Include(include);
//    }

//    return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
//}

//public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
//{
//    IQueryable<T> query = _dbSet.AsNoTracking();

//    if (filter != null)
//        query = query.Where(filter);

//    return await query.ToListAsync();
//}

//public virtual async Task<List<T>> GetAllWithIncludesAsync(Expression<Func<T, bool>>? filter = null,
//    params Expression<Func<T, object>>[] includes)
//{
//    IQueryable<T> query = _dbSet.AsNoTracking();

//    foreach (var include in includes)
//    {
//        query = query.Include(include);
//    }

//    if (filter != null)
//        query = query.Where(filter);

//    return await query.ToListAsync();
//}

//public virtual async Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
//    => await _dbSet.AsNoTracking().SingleOrDefaultAsync(predicate);

//public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
//    => await _dbSet.AnyAsync(predicate);

//public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
//{
//    if (filter == null)
//        return await _dbSet.CountAsync();

//    return await _dbSet.CountAsync(filter);
//}

//protected async Task<bool> SaveChangesAsync()
//    => await _context.SaveChangesAsync() > 0;