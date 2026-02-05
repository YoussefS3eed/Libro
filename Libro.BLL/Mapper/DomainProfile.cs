using Libro.BLL.DTOs;
using Libro.BLL.DTOs.Author;
using Libro.BLL.DTOs.Book;
using Libro.BLL.DTOs.Category;

namespace Libro.BlL.Mapper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()

        {
            // Category Mappings
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Category, SelectListItemDTO>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));


            // Author Mappings
            CreateMap<CreateAuthorDTO, Author>();
            CreateMap<UpdateAuthorDTO, Author>();
            CreateMap<Author, AuthorDto>();
            CreateMap<Author, SelectListItemDTO>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));



            // Book Mappings
            CreateMap<CreateBookDTO, Book>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore())
                .AfterMap((dto, book) =>
                {
                    foreach (var categoryId in dto.CategoryIds)
                    {
                        book.AddCategory(categoryId!.Value);
                    }
                });

            CreateMap<UpdateBookDTO, Book>();
            CreateMap<Book, BookDTO>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author!.Name))
                .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src =>
                    src.Categories.Select(c => c.CategoryId).ToList()))
                .ForMember(dest => dest.CategoryNames, opt => opt.MapFrom(src =>
                    src.Categories.Select(c => c.Category!.Name).ToList()));


        }
    }
}
