using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Libro.PL.ViewModels.Book
{
    public class BookFormViewModel
    {
        public int Id { get; set; }

        [MaxLength(500, ErrorMessage = Errors.MaxLength), Required]
        [Remote("AllowItem", null!, AdditionalFields = "Id,AuthorId", ErrorMessage = Errors.Duplicated)]
        public string Title { get; set; } = null!;

        [Display(Name = "Author"), Required]
        [Remote("AllowItem", null!, AdditionalFields = "Id,Title", ErrorMessage = Errors.Duplicated)]
        public int AuthorId { get; set; }

        public IEnumerable<SelectListItem>? Authors { get; set; }

        [MaxLength(200, ErrorMessage = Errors.MaxLength), Required]
        public string Publisher { get; set; } = null!;

        [Display(Name = "Publishing Date"), Required]
        [AssertThat("PublishingDate <= Today()", ErrorMessage = Errors.NotAllowFutureDates)]
        public DateTime PublishingDate { get; set; } = DateTime.Now;

        public IFormFile? Image { get; set; }

        public string? ImageUrl { get; set; }

        [MaxLength(50, ErrorMessage = Errors.MaxLength), Required]
        public string Hall { get; set; } = null!;

        [Display(Name = "Is available for rental?")]
        public bool IsAvailableForRental { get; set; }

        [MaxLength(2000, ErrorMessage = Errors.MaxLength), Required]
        public string Description { get; set; } = null!;

        [Display(Name = "Categories")]
        public IList<int?> SelectedCategories { get; set; } = new List<int?>();

        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}