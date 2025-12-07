using Libro.BLL.ModelVM.Author;

namespace Libro.BLL.Service.Abstraction
{
    public interface IAuthorService
    {
        Task<Response<AuthorViewModel>> CreateAuthorAsync(AuthorFormVM model);
        Task<Response<AuthorViewModel>> UpdateAuthorAsync(AuthorFormVM model);
        Task<Response<AuthorViewModel>> ToggleStatusAuthorAsync(int authorId);
        Task<Response<AuthorFormVM>> GetAuthorByIdAsync(int authorId);
        Task<Response<IEnumerable<AuthorViewModel>>> GetAllAuthorsAsync();
        Task<Response<IEnumerable<AuthorViewModel>>> GetAllNotActiveAuthorsAsync();
    }
}
