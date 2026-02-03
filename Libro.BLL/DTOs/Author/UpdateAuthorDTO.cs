namespace Libro.BLL.DTOs.Author
{
    public class UpdateAuthorDTO
    {
        public int Id { get; set; }
        [MaxLength(100), Required]
        public string Name { get; set; } = null!;
    }
}