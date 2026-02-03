namespace Libro.DAL.Entities
{
    public class Book : BaseEntity
    {
        protected Book()
        {
            CreatedBy = "Admin from Protected Book Ctor!";
        }

        public Book(string title, int authorId, string publisher, DateTime publishingDate,
                    string hall, bool isAvailableForRental, string description, string createdBy)
        {
            Title = title;
            AuthorId = authorId;
            Publisher = publisher;
            PublishingDate = publishingDate;
            Hall = hall;
            IsAvailableForRental = isAvailableForRental;
            Description = description;
            CreatedBy = createdBy;
        }

        public int Id { get; private set; }
        public string Title { get; private set; } = null!;
        public int AuthorId { get; private set; }
        public Author? Author { get; private set; }
        public string Publisher { get; private set; } = null!;
        public DateTime PublishingDate { get; private set; }
        public string? ImageUrl { get; private set; }
        public string Hall { get; private set; } = null!;
        public bool IsAvailableForRental { get; private set; }
        public string Description { get; private set; } = null!;

        private readonly List<BookCategory> _categories = new();
        public IReadOnlyCollection<BookCategory> Categories => _categories.AsReadOnly();

        //public bool Update(string title, int authorId, string publisher, DateTime publishingDate,
        //                  string hall, bool isAvailableForRental, string description, string? imageUrl, string updatedBy, List<int?> CategoryIds)
        //{
        //    Title = title;
        //    AuthorId = authorId;
        //    Publisher = publisher;
        //    PublishingDate = publishingDate;
        //    Hall = hall;
        //    IsAvailableForRental = isAvailableForRental;
        //    Description = description;
        //    ImageUrl = imageUrl;
        //    UpdatedOn = DateTime.UtcNow;
        //    UpdatedBy = updatedBy;

        //    ClearCategories();
        //    foreach (var categoryId in CategoryIds)
        //    {
        //        AddCategory((int)categoryId!);
        //    }
        //    return true;
        //}
        public bool Update(string title, int authorId, string publisher, DateTime publishingDate,
                   string hall, bool isAvailableForRental, string description,
                   string? imageUrl, string updatedBy, List<int?> categoryIds)
        {
            Title = title;
            AuthorId = authorId;
            Publisher = publisher;
            PublishingDate = publishingDate;
            Hall = hall;
            IsAvailableForRental = isAvailableForRental;
            Description = description;
            ImageUrl = imageUrl;
            UpdatedOn = DateTime.UtcNow;
            UpdatedBy = updatedBy;

            ClearCategories();
            foreach (var categoryId in categoryIds)
                AddCategory((int)categoryId!);

            return true;
        }
        public void UpdateImage(string imageUrl, string updatedBy)
        {
            ImageUrl = imageUrl;
            UpdatedOn = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }

        public void ClearCategories()
        {
            _categories.Clear();
        }

        public void AddCategory(int categoryId)
        {
            if (_categories.Any(c => c.CategoryId == categoryId))
                return;

            _categories.Add(new BookCategory(Id, categoryId));
        }

        public void ToggleStatus(string deletedBy)
        {
            if (!string.IsNullOrEmpty(deletedBy))
            {
                IsDeleted = !IsDeleted;
                DeletedBy = deletedBy;
                DeletedOn = DateTime.UtcNow;
                UpdatedOn = DateTime.UtcNow;
            }
        }
    }
}




//public bool Updated(Book newBook)
//{
//    if (Title != newBook.Title ||
//        AuthorId != newBook.AuthorId ||
//        Publisher != newBook.Publisher ||
//        PublishingDate != newBook.PublishingDate ||
//        ImageUrl != newBook.ImageUrl ||
//        Hall != newBook.Hall ||
//        IsAvailableForRental != newBook.IsAvailableForRental ||
//        Description != newBook.Description ||
//        Categories != newBook.Categories)
//    {
//        Title = newBook.Title;
//        AuthorId = newBook.AuthorId;
//        Publisher = newBook.Publisher;
//        PublishingDate = newBook.PublishingDate;
//        ImageUrl = newBook.ImageUrl;
//        Hall = newBook.Hall;
//        IsAvailableForRental = newBook.IsAvailableForRental;
//        Description = newBook.Description;
//        Categories = newBook.Categories;
//        UpdatedBy = newBook.UpdatedBy ?? "System";
//        UpdatedOn = DateTime.UtcNow;
//        return true;
//    }
//    return false;
//}
//private Book() : base()
//{
//}
//public Book(string title, int authorId, string publisher, DateTime publishingDate,
//            string hall, bool isAvailableForRental, string description, string createdBy) : base()
//{
//    Title = title;
//    AuthorId = authorId;
//    Publisher = publisher;
//    PublishingDate = publishingDate;
//    Hall = hall;
//    IsAvailableForRental = isAvailableForRental;
//    Description = description;
//    CreatedBy = createdBy;
//    CreatedOn = DateTime.UtcNow;
//}
//public int Id { get; private set; }
//public string Title { get; private set; } = null!;
//public int AuthorId { get; private set; }
//public Author? Author { get; private set; }
//public string Publisher { get; private set; } = null!;
//public DateTime PublishingDate { get; private set; }
//public string? ImageUrl { get; set; }
//public string Hall { get; private set; } = null!;
//public bool IsAvailableForRental { get; private set; }
//public string Description { get; private set; } = null!;
//public ICollection<BookCategory> Categories { get; private set; } = new List<BookCategory>();

//public bool Update(string title, int authorId, string publisher, DateTime publishingDate,
//                    string hall, bool isAvailableForRental, string description, string updatedBy)
//{
//    var hasChanges = false;

//    if (Title != title)
//    {
//        Title = title;
//        hasChanges = true;
//    }

//    if (AuthorId != authorId)
//    {
//        AuthorId = authorId;
//        hasChanges = true;
//    }

//    if (Publisher != publisher)
//    {
//        Publisher = publisher;
//        hasChanges = true;
//    }

//    if (PublishingDate != publishingDate)
//    {
//        PublishingDate = publishingDate;
//        hasChanges = true;
//    }

//    if (Hall != hall)
//    {
//        Hall = hall;
//        hasChanges = true;
//    }

//    if (IsAvailableForRental != isAvailableForRental)
//    {
//        IsAvailableForRental = isAvailableForRental;
//        hasChanges = true;
//    }

//    if (Description != description)
//    {
//        Description = description;
//        hasChanges = true;
//    }

//    if (hasChanges)
//    {
//        UpdatedOn = DateTime.UtcNow;
//        UpdatedBy = updatedBy;
//    }

//    return hasChanges;
//}

//public void UpdateImageUrl(string imageUrl, string updatedBy)
//{
//    ImageUrl = imageUrl;
//    UpdatedOn = DateTime.UtcNow;
//    UpdatedBy = updatedBy;
//}
//public bool ToggleStatus(string deletedBy)
//{
//    if (!string.IsNullOrEmpty(deletedBy))
//    {
//        IsDeleted = !IsDeleted;
//        DeletedBy = deletedBy;
//        DeletedOn = IsDeleted ? DateTime.UtcNow : null;
//        UpdatedOn = DateTime.UtcNow;
//        UpdatedBy = deletedBy;
//        return true;
//    }
//    return false;
//}
//public void UpdateCategories(ICollection<int> categoryIds)
//{
//    Categories.Clear();
//    foreach (var categoryId in categoryIds)
//    {
//        Categories.Add(new BookCategory { CategoryId = categoryId });
//    }
//}