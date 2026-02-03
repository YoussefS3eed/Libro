using Libro.BLL.DTOs.Author;
using System.Net;

namespace Libro.BLL.Service.Implementation
{
    public class AuthorService : IAuthorService, IUniqueNameValidator
    {
        private readonly IAuthorRepo _authorRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorService> _logger;
        public AuthorService(IAuthorRepo authorRepo, IMapper mapper, ILogger<AuthorService> logger)
        {
            _authorRepo = authorRepo;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Response<AuthorDto>> CreateAsync(CreateAuthorDTO dto)
        {
            try
            {
                if (dto == null)
                    return new(null, "Invalid data.", true, HttpStatusCode.BadRequest);

                if (await NameExistsAsync(dto.Name))
                    return new(null, "Author name already exists.", true, HttpStatusCode.Conflict);

                var author = _mapper.Map<Author>(dto);
                var result = await _authorRepo.AddAsync(author);

                if (result == null)
                    return new(null, "Failed to create author in database.", true, HttpStatusCode.InternalServerError);

                return new(_mapper.Map<AuthorDto>(result), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating author {Name}", dto?.Name);
                return new(null, "Unexpected error.", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<AuthorDto>> UpdateAsync(UpdateAuthorDTO dto)
        {
            try
            {
                if (dto == null)
                    return new(null, "Invalid data.", true, HttpStatusCode.BadRequest);

                var existingAuthor = await _authorRepo.GetByIdAsync(dto.Id);
                if (existingAuthor == null)
                    return new(null, "Author not found.", true, HttpStatusCode.NotFound);

                if (existingAuthor.Name != dto.Name && await NameExistsAsync(dto.Name))
                    return new(null, "Author name already exists.", true, HttpStatusCode.Conflict);

                var author = _mapper.Map<Author>(dto);
                var result = await _authorRepo.UpdateAsync(author);

                if (result == null)
                    return new(null, "Database error.", true, HttpStatusCode.BadRequest);

                return new(_mapper.Map<AuthorDto>(result), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating author {Name}", dto?.Name);
                return new(null, "Unexpected error.", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<AuthorDto>> ToggleStatusAsync(int authorId)
        {
            try
            {

                var author = await _authorRepo.GetByIdAsync(authorId);
                if (author == null)
                    return new(null, "Author not found.", true, HttpStatusCode.NotFound);

                var result = await _authorRepo.ToggleStatusAsync(author.Id);
                if (result == null)
                    return new(null, "Database error.", true, HttpStatusCode.BadRequest);

                return new(_mapper.Map<AuthorDto>(result), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status for author {Id}", authorId);
                return new(null, "Unexpected error.", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<AuthorDto>> GetByIdAsync(int authorId)
        {
            try
            {
                var author = await _authorRepo.GetByIdAsync(authorId);
                if (author == null)
                    return new(null, "Author not found.", true, HttpStatusCode.NotFound);

                return new(_mapper.Map<AuthorDto>(author), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching author by id {Id}", authorId);
                return new(null, "Could not load author.", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<IEnumerable<AuthorDto>>> GetAllAsync()
        {
            try
            {
                var authors = await _authorRepo.GetAllAsync();
                return new(_mapper.Map<IEnumerable<AuthorDto>>(authors), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active authors");
                return new(null, "Could not load authors.", true);
            }
        }
        public async Task<Response<IEnumerable<AuthorDto>>> GetAllNotActiveAsync()
        {
            try
            {
                var authors = await _authorRepo.GetAllAsync(a => a.IsDeleted);
                return new(_mapper.Map<IEnumerable<AuthorDto>>(authors), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching inactive authors");
                return new(null, "Could not load authors.", true);
            }
        }
        public async Task<bool> NameExistsAsync(string name) =>
            await _authorRepo.AnyAsync(a => a.Name == name);
        public async Task<bool> IsAllowed(int Id, string name)
        {
            var category = await _authorRepo.GetSingleOrDefaultAsync(c => c.Name == name);
            return category is null || category.Id.Equals(Id);
        }
        private string GetCurrentUser()
        {
            // TODO: Implement this to return the current user for authors
            // يجب تنفيذ هذا ليعيد المستخدم الحالي
            // مثال: يمكن استخدام IHttpContextAccessor
            return "System"; // مؤقت
        }
    }
}