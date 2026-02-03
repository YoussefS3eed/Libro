namespace Libro.DAL.Repositories.Implementation
{
    public class BookRepo : Repository<Book>, IBookRepo
    {
        private readonly LibroDbContext _context;
        public BookRepo(LibroDbContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<Book?> UpdateAsync(Book newBook)
        //{

        //    var updatedBook = await GetByIdAsync(newBook.Id);
        //    if (updatedBook is not null)
        //    {
        //        var isUpdated = updatedBook.Update(
        //        newBook.Title,
        //        newBook.AuthorId,
        //        newBook.Publisher,
        //        newBook.PublishingDate,
        //        newBook.Hall,
        //        newBook.IsAvailableForRental,
        //        newBook.Description,
        //        newBook.ImageUrl, // Image URL handled separately
        //        newBook.UpdatedBy ?? "Updated from Repo",
        //        newBook.Categories.Select(c => c.CategoryId).ToList()
        //    );
        //        if (isUpdated)
        //        {
        //            if (await SaveChangesAsync())
        //                return updatedBook;
        //        }
        //    }
        //    return null;
        //}

        public async Task<Book?> UpdateAsync(Book book1, List<int?> categoryIds)
        {
            // 1️⃣ جلب الـ Entity
            var book = await _context.Books
                .Include(b => b.Categories)
                .SingleOrDefaultAsync(b => b.Id == book1.Id);

            if (book == null || book.IsDeleted)
                return null;

            // 2️⃣ تعديل باستخدام Entity logic
            book.Update(book1.Title, book1.AuthorId, book1.Publisher, book1.PublishingDate, book1.Hall,
                        book1.IsAvailableForRental, book1.Description, book1.ImageUrl, book1.UpdatedBy!, categoryIds);

            // 3️⃣ حفظ التغييرات
            var success = await _context.SaveChangesAsync();
            return success > 0 ? book : null;
        }


        public async Task<Book?> GetByIdWithCategoriesAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Categories)
                .SingleOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book?> GetByIdWithAuthorAndCategoriesAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .SingleOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetByAuthorIdAsync(int authorId)
        {
            return await _context.Books
                .Where(b => b.AuthorId == authorId && !b.IsDeleted)
                .ToListAsync();
        }
    }
}



//private readonly LibroDbContext _context;
//public BookRepo(LibroDbContext context) : base(context) { }

//public async Task<Book?> GetByIdWithDetailsAsync(int id)
//{
//    return await _context.Books
//        .Include(b => b.Categories)
//        .Include(b => b.Author)
//        .FirstOrDefaultAsync(b => b.Id == id);
//}

//public async Task<IEnumerable<Book>> GetAllWithDetailsAsync()
//{
//    return await _context.Books
//        .Include(b => b.Categories)
//        .Include(b => b.Author)
//        .ToListAsync();
//}

//public async Task<bool> ExistsByTitleAndAuthorAsync(string title, int authorId, int? excludeId = null)
//{
//    var query = _context.Books
//        .Where(b => b.Title == title && b.AuthorId == authorId);

//    if (excludeId.HasValue)
//    {
//        query = query.Where(b => b.Id != excludeId.Value);
//    }

//    return await query.AnyAsync();
//}

























//private readonly LibroDbContext _context;
//public BookRepo(LibroDbContext context) : base(context)
//{
//    _context = context;
//}
//public async Task<Book?> GetBookWithCategoriesAsync(int id)
//{
//    return await _context.Books
//        .Include(b => b.Categories)
//        .Include(b => b.Author)
//        .FirstOrDefaultAsync(b => b.Id == id);
//}
//public async Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync()
//{
//    return await _context.Books
//        .Include(b => b.Author)
//        .Include(b => b.Categories)
//        .ThenInclude(bc => bc.Category)
//        .Where(b => !b.IsDeleted)
//        .ToListAsync();
//}
//public async Task<bool> ExistsByTitleAndAuthorAsync(string title, int authorId, int? excludeId = null)
//{
//    var query = _context.Books
//        .Where(b => b.Title == title && b.AuthorId == authorId && !b.IsDeleted);

//    if (excludeId.HasValue)
//    {
//        query = query.Where(b => b.Id != excludeId.Value);
//    }

//    return await query.AnyAsync();
//}

//public async Task<Book?> UpdateAsync(Book newBook)
//{
//    var updatedBook = await GetByIdAsync(newBook.Id);
//    if (updatedBook is not null)
//    {
//        if (updatedBook.Updated(newBook))
//        {
//            if (await SaveChangesAsync())
//                return updatedBook;
//        }
//    }
//    return null;
//}
