namespace Libro.DAL.Repo.Implementation
{
    public class AuthorRepo : IAuthorRepo
    {
        private readonly LibroDbContext _context;

        public AuthorRepo(LibroDbContext context)
        {
            _context = context;
        }
        private async Task<bool> SaveChangesAsync()
            => await _context.SaveChangesAsync() > 0;
        public async Task<Author?> AddAsync(Author author)
        {
            try
            {
                await _context.Authors.AddAsync(author);
                if (await SaveChangesAsync())
                    return author;
                return null;
            }
            catch
            {
                throw;
            }
        }
        public async Task<Author?> UpdateAsync(Author newAuthor)
        {
            try
            {
                var updatedAuthor = await GetAuthorByIdAsync(newAuthor.Id);
                if (updatedAuthor is not null)
                {
                    var isUpdated = updatedAuthor.Update(newAuthor.Name, newAuthor.UpdatedBy! ?? "System");
                    if (isUpdated)
                    {
                        if (await SaveChangesAsync())
                            return updatedAuthor;
                    }
                }
                return null;
            }
            catch
            {
                throw;
            }
        }
        public async Task<Author?> ToggleStatusAsync(int id)
        {
            try
            {
                var author = await GetAuthorByIdAsync(id);
                if (author is null)
                {
                    return null;
                }

                author.ToggleStatus("System");
                if (await SaveChangesAsync())
                    return author;
                return null;
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> AnyAsync(Expression<Func<Author, bool>> predicate)
            => await _context.Authors.AnyAsync(predicate);
        public async Task<Author?> GetAuthorByIdAsync(int id)
            => await _context.Authors.FindAsync(id);
        public IQueryable<Author> GetAllAuthors(Expression<Func<Author, bool>>? filter = null)
        {
            try
            {
                IQueryable<Author> query = _context.Authors;

                if (filter is not null)
                {
                    query = query.Where(filter);
                }

                return query;
            }
            catch
            {
                throw;
            }
        }
    }
}