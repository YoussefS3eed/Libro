using AutoMapper;
using Libro.BLL.DTOs;
using Libro.BLL.DTOs.Author;
using Libro.BLL.DTOs.Book;
using Libro.BLL.DTOs.Category;
using Libro.DAL.Entities;
using Libro.PL.ViewModels.Author;
using Libro.PL.ViewModels.Book;
using Libro.PL.ViewModels.Category;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Libro.PL.Mapper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<CategoryDto, CategoryFormViewModel>();
            CreateMap<CategoryDto, CategoryViewModel>();
            CreateMap<CategoryFormViewModel, CreateCategoryDTO>();
            CreateMap<CategoryFormViewModel, UpdateCategoryDTO>();

            CreateMap<AuthorDto, AuthorFormViewModel>();
            CreateMap<AuthorDto, AuthorViewModel>();
            CreateMap<AuthorFormViewModel, CreateAuthorDTO>();
            CreateMap<AuthorFormViewModel, UpdateAuthorDTO>();


            // Map from BLL DTOs to ViewModels
            CreateMap<BookDTO, BookFormViewModel>()
                .ForMember(dest => dest.SelectedCategories, opt => opt.MapFrom(src => src.CategoryIds))
                .ForMember(dest => dest.Authors, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.Ignore());


            // Map from ViewModels to BLL DTOs
            CreateMap<BookFormViewModel, CreateBookDTO>()
                .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.SelectedCategories))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(_ => "System from mapper"));

            CreateMap<BookFormViewModel, UpdateBookDTO>()
                .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.SelectedCategories))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(_ => "System System from mapper"));

            CreateMap<SelectListItemDTO, SelectListItem>();

            CreateMap<BookDTO, BookViewModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.AuthorName))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CategoryNames));
                
        }
    }
}
