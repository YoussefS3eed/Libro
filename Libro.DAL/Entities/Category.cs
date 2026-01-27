namespace Libro.DAL.Entities
{
    public class Category : BaseEntity
    {
        protected Category()
        {
            CreatedBy = "Admin Category";
        }
        public Category(string name, string createdBy)
        {
            Name = name;
            CreatedBy = createdBy;
        }
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public ICollection<BookCategory> Books { get; private set; } = new List<BookCategory>();
        public bool Update(string name, string deletedBy)
        {
            if (!string.IsNullOrEmpty(deletedBy) && Name != name)
            {
                Name = name;
                UpdatedOn = DateTime.UtcNow;
                UpdatedBy = deletedBy;
                return true;
            }
            return false;
        }
        public bool ToggleStatus(string deletedBy)
        {
            if (!string.IsNullOrEmpty(deletedBy))
            {
                IsDeleted = !IsDeleted;
                DeletedBy = deletedBy;
                DeletedOn = DateTime.UtcNow;
                UpdatedOn = DateTime.UtcNow;
                return true;
            }
            return false;
        }
    }
}
