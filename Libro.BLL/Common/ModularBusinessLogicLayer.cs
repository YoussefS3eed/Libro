using Libro.BlL.Mapper;
namespace Libro.BLL.Common
{
    public static class ModularBusinessLogicLayer
    {
        public static IServiceCollection AddBusinessLogicLayerInPL(this IServiceCollection services)
        {
            services.AddScoped<CategoryService>();
            services.AddScoped<IUniqueNameValidator, CategoryService>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<AuthorService>();
            services.AddScoped<IUniqueNameValidator, AuthorService>();
            services.AddScoped<IAuthorService, AuthorService>();

            services.AddScoped<IBookService, BookService>();

            services.AddAutoMapper(x => x.AddProfile<DomainProfile>());
            return services;
        }
    }
}
