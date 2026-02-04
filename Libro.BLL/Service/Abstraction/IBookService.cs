using Libro.BLL.DTOs;
using Libro.BLL.DTOs.Book;
namespace Libro.BLL.Service.Abstraction
{
    public interface IBookService
    {
        Task<Response<BookDTO>> GetByIdAsync(int bookId);
        Task<Response<IEnumerable<BookDTO>>> GetAllAsync();
        Task<Response<BookDTO>> GetByIdWithAuthorAndCategoriesAsync(int id);
        Task<Response<BookDTO>> CreateAsync(CreateBookDTO dto);
        Task<Response<BookDTO>> UpdateAsync(UpdateBookDTO dto);
        Task<Response<bool>> ToggleStatusAsync(int id, string deletedBy);
        Task<Response<IEnumerable<SelectListItemDTO>>> GetActiveAuthorsForDropdownAsync();
        Task<Response<IEnumerable<SelectListItemDTO>>> GetActiveCategoriesForDropdownAsync();
        Task<bool> IsAllowed(int Id, string Title, int AuthorId);
    }
}