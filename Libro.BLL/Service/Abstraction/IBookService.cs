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
    }
}

//Task<Response<BookDTO>> GetByIdAsync(int id);
//Task<Response<IEnumerable<BookDTO>>> GetAllAsync();
//Task<Response<BookDTO>> CreateAsync(CreateBookDTO dto, string createdBy);
//Task<Response<BookDTO>> UpdateAsync(UpdateBookDTO dto, string updatedBy);
//Task<Response<bool>> DeleteAsync(int id, string deletedBy);
//Task<Response<bool>> ToggleStatusAsync(int id, string deletedBy);


//Task<BookDTO?> GetBookByIdAsync(int id);
//Task<IEnumerable<BookDTO>> GetAllBooksAsync();
//Task<BookDTO> CreateBookAsync(CreateBookDTO dto, string currentUserId);
//Task<BookDTO?> UpdateBookAsync(UpdateBookDTO dto, string currentUserId);
//Task<bool> DeleteBookAsync(int id, string currentUserId);
//Task<IEnumerable<SelectListItemDTO>> GetAuthorsForDropdownAsync();
//Task<IEnumerable<SelectListItemDTO>> GetCategoriesForDropdownAsync();
//Task<bool> IsBookTitleUniqueAsync(string title, int authorId, int? excludeId = null);