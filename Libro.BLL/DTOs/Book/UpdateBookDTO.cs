namespace Libro.BLL.DTOs.Book
{
    public class UpdateBookDTO
    {
        [Required]
        public int Id { get; set; }
        [MaxLength(500), Required]
        public string Title { get; set; } = null!;
        [Required]
        public int AuthorId { get; set; }
        [MaxLength(200), Required]
        public string Publisher { get; set; } = null!;
        [Required]
        public DateTime PublishingDate { get; set; }
        public string? ImageUrl { get; set; }
        [MaxLength(50), Required]
        public string Hall { get; set; } = null!;
        public bool IsAvailableForRental { get; set; }
        [MaxLength(2000), Required]
        public string Description { get; set; } = null!;
        public List<int?> CategoryIds { get; set; } = new();
        [Required]
        public string UpdatedBy { get; set; } = null!;
    }
}

//public int Id { get; set; }

//[Required]
//[MaxLength(500)]
//public string Title { get; set; } = null!;

//[Required]
//public int AuthorId { get; set; }

//[Required]
//[MaxLength(200)]
//public string Publisher { get; set; } = null!;

//[Required]
//public DateTime PublishingDate { get; set; }

//[MaxLength(50)]
//public string Hall { get; set; } = null!;

//public bool IsAvailableForRental { get; set; }

//[Required]
//public string Description { get; set; } = null!;

//public byte[]? ImageBytes { get; set; }
//public string? ImageExtension { get; set; }
//public bool RemoveImage { get; set; }
//public List<int> CategoryIds { get; set; } = new();