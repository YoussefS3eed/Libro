using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;


namespace Libro.DAL.Database
{
    public class LibroDbContext : IdentityDbContext
    {
        public LibroDbContext(DbContextOptions<LibroDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
