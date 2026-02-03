namespace Libro.DAL.Entities.Base
{
    public class BaseEntity
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string? DeletedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
