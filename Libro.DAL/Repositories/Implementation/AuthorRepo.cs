namespace Libro.DAL.Repositories.Implementation
{
    public class AuthorRepo : Repository<Author>, IAuthorRepo
    {
        private readonly LibroDbContext _context;
        public AuthorRepo(LibroDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Author?> UpdateAsync(Author newAuthor)
        {

            var updatedAuthor = await GetByIdAsync(newAuthor.Id);
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
        public async Task<Author?> ToggleStatusAsync(int id)
        {
            var author = await GetByIdAsync(id);
            author!.ToggleStatus("System");
            if (await SaveChangesAsync())
                return author;
            return null;
        }
    }
}