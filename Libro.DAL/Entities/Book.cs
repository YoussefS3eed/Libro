namespace Libro.DAL.Entities
{
    public class Book : BaseEntity
    {
        protected Book()
        {
            CreatedBy = "Admin from Protected Book Ctor!";
        }

        public Book(
            string title,
            int authorId,
            string publisher,
            DateTime publishingDate,
            string hall,
            bool isAvailableForRental,
            string description,
            string createdBy,
            IEnumerable<int>? categoryIds = null)
        {
            Title = title;
            AuthorId = authorId;
            Publisher = publisher;
            PublishingDate = publishingDate;
            Hall = hall;
            IsAvailableForRental = isAvailableForRental;
            Description = description;

            CreatedBy = createdBy;
            CreatedOn = DateTime.UtcNow;

            if (categoryIds != null)
            {
                foreach (var categoryId in categoryIds.Distinct())
                    AddCategory(categoryId);
            }
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

        public bool Update(string title, int authorId, string publisher, DateTime publishingDate,
                    string hall, bool isAvailableForRental, string description,
                    string? imageUrl, string updatedBy, IEnumerable<int?> categoryIds)
        {
            var changed = false;
            if (Title != title ||
                    AuthorId != authorId ||
                    Publisher != publisher ||
                    PublishingDate != publishingDate ||
                    ImageUrl != imageUrl ||
                    Hall != hall ||
                    IsAvailableForRental != isAvailableForRental ||
                    Description != description)
            {
                Title = title;
                AuthorId = authorId;
                Publisher = publisher;
                PublishingDate = publishingDate;
                Hall = hall;
                IsAvailableForRental = isAvailableForRental;
                Description = description;
                ImageUrl = imageUrl;
                changed = true;
            }

            if (UpdateCategories(categoryIds))
                changed = true;

            if (changed)
            {
                UpdatedOn = DateTime.UtcNow;
                UpdatedBy = updatedBy;
            }
            return changed;
        }

        public void UpdateImage(string imageUrl, string updatedBy)
        {
            if (ImageUrl == imageUrl)
                return;

            ImageUrl = imageUrl;
            UpdatedOn = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }

        private bool UpdateCategories(IEnumerable<int?> categoryIds)
        {
            var newIds = categoryIds
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var existingIds = _categories
                .Select(x => x.CategoryId)
                .OrderBy(x => x)
                .ToList();

            if (existingIds.SequenceEqual(newIds))
                return false;

            _categories.Clear();

            foreach (var categoryId in newIds)
                AddCategory(categoryId);

            return true;
        }

        public void AddCategory(int? categoryId)
        {
            if (_categories.Any(c => c.CategoryId == categoryId))
                return;

            _categories.Add(new BookCategory(categoryId));
        }

        public void ToggleStatus(string actionBy)
        {
            IsDeleted = !IsDeleted;

            DeletedBy = IsDeleted ? actionBy : null;
            DeletedOn = IsDeleted ? DateTime.UtcNow : null;

            UpdatedOn = DateTime.UtcNow;
            UpdatedBy = actionBy;
        }
    }
}