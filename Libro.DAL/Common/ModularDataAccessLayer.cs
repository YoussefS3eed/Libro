using Libro.DAL.Repositories.Implementation;
namespace Libro.DAL.Common
{
    public static class ModularDataAccessLayer
    {
        public static IServiceCollection AddDataAccessLayerInPL(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IAuthorRepo, AuthorRepo>();
            services.AddScoped<ICategoryRepo, CategoryRepo>();
            services.AddScoped<IBookRepo, BookRepo>();
            return services;
        }
    }
}
