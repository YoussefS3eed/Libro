namespace Libro.DAL.Repositories.Implementation
{
    public class CategoryRepo : Repository<Category>, ICategoryRepo
    {
        private readonly LibroDbContext _context;
        public CategoryRepo(LibroDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Category?> UpdateAsync(Category newCategory)
        {
            var updatedCategory = await GetByIdAsync(newCategory.Id);
            if (updatedCategory is not null)
            {
                var isUpdated = updatedCategory.Update(newCategory.Name, newCategory.UpdatedBy! ?? "System");
                if (isUpdated)
                {
                    if (await SaveChangesAsync())
                        return updatedCategory;
                }
            }
            return null;
        }
        public async Task<Category?> ToggleStatusAsync(int id)
        {
            var category = await GetByIdAsync(id);
            category!.ToggleStatus("System");
            if (await SaveChangesAsync())
                return category;
            return null;
        }
    }
}
