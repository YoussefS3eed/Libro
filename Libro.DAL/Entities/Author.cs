namespace Libro.DAL.Entities
{
    public class Author : BaseEntity
    {
        protected Author()
        {
            CreatedBy = "Admin from protected Author ctor";
        }
        public Author(string name, string createdBy)
        {
            Name = name;
            CreatedBy = createdBy;
        }
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public bool Update(string name, string createdBy)
        {
            if (!string.IsNullOrEmpty(createdBy) && Name != name)
            {
                Name = name;
                UpdatedOn = DateTime.UtcNow;
                UpdatedBy = createdBy;
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
