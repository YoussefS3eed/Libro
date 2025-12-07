using Libro.BLL.ModelVM.Author;
using System.Net;

namespace Libro.BLL.Service.Implementation
{
    public class AuthorService : IAuthorService, IUniqueNameValidator
    {
        public readonly IAuthorRepo _authorRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorService> _logger;
        public AuthorService(IAuthorRepo authorRepo, IMapper mapper, ILogger<AuthorService> logger)
        {
            _authorRepo = authorRepo;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Response<AuthorViewModel>> CreateAuthorAsync(AuthorFormVM model)
        {
            try
            {
                if (model is null)
                    return new(null, "You don't enter a write values when you creating author are you atker!!", true, HttpStatusCode.NotAcceptable);

                var mappingAuthor = _mapper.Map<Author>(model);
                var newAuthor = await _authorRepo.AddAsync(mappingAuthor);
                if (newAuthor is null)
                {
                    return new(null, "something went wrong when we adding in database", true, HttpStatusCode.InternalServerError);
                }
                var newAuthorViewModel = _mapper.Map<AuthorViewModel>(newAuthor);
                return new(newAuthorViewModel, null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while adding the author {Name} with error massage: {ErrorMassage}", model?.Name, ex.Message);
                return new(null, "An error occurred while adding the author.", true, HttpStatusCode.GatewayTimeout);
            }
        }

        public async Task<Response<AuthorViewModel>> UpdateAuthorAsync(AuthorFormVM model)
        {
            try
            {
                if (model is null)
                    return new(null, "You don't enter a write values when you editing author are you atker!!", true, HttpStatusCode.NotAcceptable);

                var mappingAuthor = _mapper.Map<Author>(model);
                var updateAuthor = await _authorRepo.UpdateAsync(mappingAuthor);
                if (updateAuthor is null)
                {
                    return new(null, "You must change your author name", true, HttpStatusCode.BadRequest);
                }

                var updatedAuthorViewModel = _mapper.Map<AuthorViewModel>(updateAuthor);
                return new(updatedAuthorViewModel, null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating the author {Name} with error massage: {ErrorMassage}", model?.Name, ex.Message);
                return new(null, "An error occurred while updating the author.", true, HttpStatusCode.GatewayTimeout);
            }
        }
        public async Task<Response<AuthorViewModel>> ToggleStatusAuthorAsync(int authorId)
        {
            try
            {
                var author = await _authorRepo.ToggleStatusAsync(authorId);
                if (author is null)
                {
                    return new(null, "Not Found Your Author or soething went wrong when we save in database", true, HttpStatusCode.GatewayTimeout);
                }
                var authorViewModel = _mapper.Map<AuthorViewModel>(author);
                return new(authorViewModel, null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while toggling the author status with id {ID} with error massage: {ErrorMassage}", authorId, ex.Message);
                return new(null, "An error occurred while toggling the author status.", true, HttpStatusCode.BadRequest);
            }
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _authorRepo.AnyAsync(c => c.Name == name);
        }
        public async Task<Response<AuthorFormVM>> GetAuthorByIdAsync(int authorId)
        {
            try
            {
                var author = await _authorRepo.GetAuthorByIdAsync(authorId);
                if (author is null)
                {
                    return new(null, "Author not found.", true);
                }
                return new(_mapper.Map<AuthorFormVM>(author), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching author by id {ID}: {Message}", authorId, ex.Message);
                return new(null, "Could not load author.", true);
            }
        }
        public async Task<Response<IEnumerable<AuthorViewModel>>> GetAllAuthorsAsync()
        {
            try
            {
                var authors = await _authorRepo.GetAllAuthors().AsNoTracking().ToListAsync();
                var mappedAuthors = _mapper.Map<IEnumerable<AuthorViewModel>>(authors);
                return new(mappedAuthors, null, false);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching active authors: {Message}", ex.Message);
                return new(null, "Could not load authors.", true);
            }
        }

        public async Task<Response<IEnumerable<AuthorViewModel>>> GetAllNotActiveAuthorsAsync()
        {
            try
            {
                var authors = await _authorRepo.GetAllAuthors(x => x.IsDeleted).AsNoTracking().ToListAsync();
                var mappedAuthors = _mapper.Map<IEnumerable<AuthorViewModel>>(authors);
                return new(mappedAuthors, null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching inactive authors: {Message}", ex.Message);
                return new(null, "Could not load authors.", true);
            }
        }
    }
}