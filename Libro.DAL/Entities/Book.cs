namespace Libro.DAL.Entities
{
    public class Book : BaseEntity
    {
        protected Book()
        {
            CreatedBy = "Admin Author";
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
        public ICollection<BookCategory> Categories { get; private set; } = new List<BookCategory>();
    }
}
