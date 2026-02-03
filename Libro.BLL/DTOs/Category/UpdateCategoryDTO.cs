namespace Libro.BLL.DTOs.Category
{
    public class UpdateCategoryDTO
    {
        public int Id { get; set; }
        [MaxLength(100), Required]
        public string Name { get; set; } = null!;
    }
}
