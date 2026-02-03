namespace Libro.BLL.DTOs.Author
{
    public class CreateAuthorDTO
    {
        [MaxLength(100), Required]
        public string Name { get; set; } = null!;
    }
}
