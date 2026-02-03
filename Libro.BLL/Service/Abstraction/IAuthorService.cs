using Libro.BLL.DTOs.Author;

namespace Libro.BLL.Service.Abstraction
{
    public interface IAuthorService
    {
        Task<Response<AuthorDto>> CreateAsync(CreateAuthorDTO dto);
        Task<Response<AuthorDto>> UpdateAsync(UpdateAuthorDTO dto);
        Task<Response<AuthorDto>> ToggleStatusAsync(int authorId);
        Task<Response<AuthorDto>> GetByIdAsync(int authorId);
        Task<Response<IEnumerable<AuthorDto>>> GetAllAsync();
        Task<Response<IEnumerable<AuthorDto>>> GetAllNotActiveAsync();
        Task<bool> IsAllowed(int Id, string Name);
    }
}
