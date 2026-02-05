using Libro.BLL.DTOs;
using Libro.BLL.DTOs.Book;
using System.Net;
namespace Libro.BLL.Service.Implementation
{
    public class BookService : IBookService
    {
        private readonly IBookRepo _bookRepo;
        private readonly IRepository<Author> _authorRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> _logger;

        public BookService(
            IBookRepo bookRepository,
            IRepository<Author> authorRepository,
            IRepository<Category> categoryRepository,
            IMapper mapper,
            ILogger<BookService> logger)
        {
            _bookRepo = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Response<BookDTO>> GetByIdAsync(int bookId)
        {
            try
            {
                var book = await _bookRepo.GetByIdAsync(bookId);
                if (book == null)
                    return new(null, "Book not found.", true, HttpStatusCode.NotFound);

                return new(_mapper.Map<BookDTO>(book), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching book by id {Id}", bookId);
                return new(null, "Could not load book.", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<BookDTO>> GetByIdWithAuthorAndCategoriesAsync(int id)
        {
            try
            {
                var book = await _bookRepo.GetByIdWithAuthorAndCategoriesAsync(id);
                if (book == null || book.IsDeleted)
                    return new(null, "Book not found", true, HttpStatusCode.NotFound);

                return new(_mapper.Map<BookDTO>(book), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book by ID {Id}", id);
                return new(null, $"Error retrieving book: {ex.Message}", true, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response<BookDTO>> GetByIdWithAuthorAndCategoriesAndCategoryAsync(int id)
        {
            try
            {
                var book = await _bookRepo.GetByIdWithAuthorAndCategoriesAndCategoryAsync(id);
                if (book == null)
                    return new(null, "Book not found", true, HttpStatusCode.NotFound);
                var maped = _mapper.Map<BookDTO>(book);
                return new(maped, null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book by ID {Id}", id);
                return new(null, $"Error retrieving book: {ex.Message}", true, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response<IEnumerable<BookDTO>>> GetAllAsync()
        {
            var books = await _bookRepo.GetAllAsync(b => !b.IsDeleted);
            var dtos = _mapper.Map<IEnumerable<BookDTO>>(books);

            return new(dtos, null, false);
        }
        public async Task<Response<BookDTO>> CreateAsync(CreateBookDTO dto)
        {
            try
            {
                // Check for duplicate book
                var exists = await ExistsByTitleAndAuthorAsync(dto.Title, dto.AuthorId);
                if (exists)
                    return new(null, "Book with same title and author already exists", true, HttpStatusCode.Conflict);

                var book = _mapper.Map<Book>(dto);

                // Save book
                var createdBook = await _bookRepo.AddAsync(book);
                if (createdBook == null)
                    return new Response<BookDTO>(null, "Failed to create book", true, HttpStatusCode.InternalServerError);

                return new(_mapper.Map<BookDTO>(createdBook), null, false, HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return new(null, $"Error creating book: {ex.Message}", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<BookDTO>> UpdateAsync(UpdateBookDTO dto)
        {
            // TODO: Make UnitOfWork And Remake this ys4s chatgpt مراجعة موديل Book
            // Validate Author
            var author = await _authorRepository.GetByIdAsync(dto.AuthorId);
            if (author == null || author.IsDeleted)
                return new(null, "Author not found", true, HttpStatusCode.BadRequest);

            // Call Repository UpdateAsync
            var updatedBook = await _bookRepo.UpdateAsync(_mapper.Map<Book>(dto), dto.CategoryIds);
            if (updatedBook == null)
                return new(null, "Failed to update book", true, HttpStatusCode.BadRequest);

            // 4️⃣ Map to DTO
            var resultDto = _mapper.Map<BookDTO>(updatedBook);
            return new(resultDto, null, false);
        }
        public async Task<Response<bool>> ToggleStatusAsync(int id, string deletedBy)
        {
            var book = await _bookRepo.GetByIdAsync(id);
            if (book == null)
                return new Response<bool>(false, "Book not found", true, HttpStatusCode.NotFound);

            book.ToggleStatus(deletedBy);
            await _bookRepo.SaveChangesAsync();

            return new Response<bool>(true, null, false);
        }
        public async Task<Response<IEnumerable<SelectListItemDTO>>> GetActiveAuthorsForDropdownAsync()
        {
            try
            {
                var authors = await _authorRepository.GetAllAsync(a => !a.IsDeleted);
                var dtos = _mapper.Map<IEnumerable<SelectListItemDTO>>(authors.OrderBy(a => a.Name));
                return new(dtos, null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving authors for dropdown");
                return new(null, $"Error retrieving authors: {ex.Message}", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<IEnumerable<SelectListItemDTO>>> GetActiveCategoriesForDropdownAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync(c => !c.IsDeleted);
                var dtos = _mapper.Map<IEnumerable<SelectListItemDTO>>(categories.OrderBy(c => c.Name));
                return new(dtos, null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories for dropdown");
                return new(null, $"Error retrieving categories: {ex.Message}", true, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<bool> IsAllowed(int Id, string Title, int AuthorId)
        {
            var book = await _bookRepo.GetSingleOrDefaultAsync(b => b.Title == Title && b.AuthorId == AuthorId);
            return book is null || book.Id.Equals(Id);
        }
        private async Task<bool> ExistsByTitleAndAuthorAsync(string title, int authorId)
        => await _bookRepo.AnyAsync(b => b.Title == title && b.AuthorId == authorId && !b.IsDeleted);
    }
}