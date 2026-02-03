using Libro.BLL.DTOs;
using Libro.BLL.DTOs.Author;
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

                var resultDto = _mapper.Map<BookDTO>(createdBook);
                return new (resultDto, null, false, HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return new (null, $"Error creating book: {ex.Message}", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<BookDTO>> UpdateAsync(UpdateBookDTO dto)
        {
            // Validate Author
            var author = await _authorRepository.GetByIdAsync(dto.AuthorId);
            if (author == null || author.IsDeleted)
                return new(null, "Author not found", true, HttpStatusCode.BadRequest);

            // 3️⃣ Call Repository UpdateAsync
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
        private async Task<bool> ExistsByTitleAndAuthorAsync(string title, int authorId)
        => await _bookRepo.AnyAsync(b => b.Title == title && b.AuthorId == authorId && !b.IsDeleted);
    }
}

//private readonly IBookRepo _bookRepo;
//private readonly IAuthorRepo _authorRepo;
//private readonly ICategoryRepo _categoryRepo;
//private readonly IMapper _mapper;
//private readonly IImageService _imageService;
//private readonly ILogger<BookService> _logger;

//public BookService(
//    IBookRepo bookRepository,
//    IAuthorRepo authorRepository,
//    ICategoryRepo categoryRepository,
//    IMapper mapper,
//    IImageService imageService,
//    ILogger<AuthorService> logger)
//{
//    _bookRepo = bookRepository;
//    _authorRepo = authorRepository;
//    _categoryRepo = categoryRepository;
//    _mapper = mapper;
//    _imageService = imageService;
//    _logger = logger;
//}
//public async Task<BookDTO?> GetBookByIdAsync(int id)
//{
//    try
//    {
//        var book = await _bookRepo.GetBookWithCategoriesAsync(id);
//        if (book is null)
//        {
//            return null;
//        }
//        return _mapper.Map<BookDTO>(book);
//    }
//    catch (Exception ex)
//    {
//        _logger.LogError(ex, "Error to get book with categories {id}", id);
//        return null;
//    }
//}

//public async Task<IEnumerable<BookDTO>> GetAllBooksAsync()
//{
//    try
//    {
//        var books = await _bookRepo.GetAllBooksWithDetailsAsync();
//        return _mapper.Map<IEnumerable<BookDTO>>(books);
//    }
//    catch (Exception ex)
//    {
//        _logger.LogError(ex, "Error to get all books with details");
//        return Enumerable.Empty<BookDTO>();
//    }
//}

//public async Task<BookDTO> CreateBookAsync(CreateBookDTO dto, string currentUserId)
//{
//    // Validate business rules
//    if (await _bookRepo.ExistsByTitleAndAuthorAsync(dto.Title, dto.AuthorId))
//    {
//        throw new ($"A book with title '{dto.Title}' already exists for this author.");
//    }

//    // Map to entity
//    var book = _mapper.Map<Book>(dto);

//    // Set audit fields
//    book.CreatedBy = currentUserId;
//    book.CreatedOn = DateTime.UtcNow;

//    // Handle image
//    if (dto.ImageBytes != null && !string.IsNullOrEmpty(dto.ImageExtension))
//    {
//        book.ImageUrl = await _imageService.SaveImageAsync(dto.ImageBytes, dto.ImageExtension, "books");
//    }

//    // Add categories
//    foreach (var categoryId in dto.CategoryIds)
//    {
//        book.Categories.Add(new BookCategory { CategoryId = categoryId });
//    }

//    // Save
//    var createdBook = await _bookRepo.AddAsync(book);
//    if (createdBook == null)
//    {
//        throw new ("Failed to create book.");
//    }

//    return _mapper.Map<BookDTO>(createdBook);
//}

//public async Task<BookDTO?> UpdateBookAsync(UpdateBookDTO dto, string currentUserId)
//{
//    var book = await _bookRepo.GetBookWithCategoriesAsync(dto.Id);
//    if (book == null)
//    {
//        return null;
//    }

//    // Validate business rules
//    if (await _bookRepo.ExistsByTitleAndAuthorAsync(dto.Title, dto.AuthorId, dto.Id))
//    {
//        throw new ($"A book with title '{dto.Title}' already exists for this author.");
//    }

//    // Update properties
//    _mapper.Map(dto, book);

//    // Set audit fields
//    book.UpdatedBy = currentUserId;
//    book.UpdatedOn = DateTime.UtcNow;

//    // Handle image
//    if (dto.RemoveImage && !string.IsNullOrEmpty(book.ImageUrl))
//    {
//        await _imageService.DeleteImageAsync(book.ImageUrl, "books");
//        book.ImageUrl = null;
//    }
//    else if (dto.ImageBytes != null && !string.IsNullOrEmpty(dto.ImageExtension))
//    {
//        // Delete old image if exists
//        if (!string.IsNullOrEmpty(book.ImageUrl))
//        {
//            await _imageService.DeleteImageAsync(book.ImageUrl, "books");
//        }

//        // Save new image
//        book.ImageUrl = await _imageService.SaveImageAsync(dto.ImageBytes, dto.ImageExtension, "books");
//    }

//    // Update categories
//    book.Categories.Clear();
//    foreach (var categoryId in dto.CategoryIds)
//    {
//        book.Categories.Add(new BookCategory { BookId = dto.Id, CategoryId = categoryId });
//    }

//    await _bookRepo.UpdateAsync(book);

//    return _mapper.Map<BookDTO>(book);
//}

//public async Task<bool> DeleteBookAsync(int id, string currentUserId)
//{
//    var book = await _bookRepo.GetByIdAsync(id);
//    if (book == null)
//    {
//        return false;
//    }

//    book.IsDeleted = true;
//    book.DeletedBy = currentUserId;
//    book.DeletedOn = DateTime.UtcNow;

//    await _bookRepo.UpdateAsync(book);
//    return true;
//}

//public async Task<IEnumerable<SelectListItemDTO>> GetAuthorsForDropdownAsync()
//{
//    var authors = await _authorRepo.GetAllAsync(a => !a.IsDeleted);
//    return _mapper.Map<IEnumerable<SelectListItemDTO>>(authors);
//}

//public async Task<IEnumerable<SelectListItemDTO>> GetCategoriesForDropdownAsync()
//{
//    var categories = await _categoryRepo.GetAllAsync(c => !c.IsDeleted);
//    return _mapper.Map<IEnumerable<SelectListItemDTO>>(categories);
//}

//public async Task<bool> IsBookTitleUniqueAsync(string title, int authorId, int? excludeId = null)
//{
//    return !await _bookRepo.ExistsByTitleAndAuthorAsync(title, authorId, excludeId);
//}