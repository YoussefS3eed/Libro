namespace Libro.BLL.DTOs.Category
{
    public class CreateCategoryDTO
    {
        [MaxLength(100), Required]
        public string Name { get; set; } = null!;
    }
}
