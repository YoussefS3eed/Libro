using AutoMapper;
using CloudinaryDotNet;
using Libro.BLL.DTOs.Book;
using Libro.DAL.Entities;
using Libro.PL.Settings;
using Libro.PL.ViewModels.Book;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Libro.PL.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //private readonly Cloudinary _cloudinary;

        private readonly List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private const int _maxAllowedSize = 2097152; // 2MB

        public BooksController(IBookService bookService, IMapper mapper,
            IWebHostEnvironment webHostEnvironment, IOptions<CloudinarySettings> cloudinary)
        {
            _bookService = bookService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;

            //_cloudinary = new Cloudinary(new Account(cloudinary.Value.Cloud, cloudinary.Value.ApiKey, cloudinary.Value.ApiSecret));
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int id)
        {

            var book = await _bookService.GetByIdWithAuthorAndCategoriesAndCategoryAsync(id);
            if (book is null || book.Result is null)
                return NotFound();

            var viewModel = _mapper.Map<BookViewModel>(book.Result);

            return View(viewModel);
        }
        public async Task<IActionResult> Create()
        {
            return View("Form", await PopulateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", await PopulateViewModel(model));

            // Handle image upload
            if (model.Image != null)
            {
                var uploadImageResult = await HandleImageUpload(model.Image);
                if (!uploadImageResult.Success)
                {
                    ModelState.AddModelError(nameof(model.Image), uploadImageResult.Error!);
                    return View("Form", await PopulateViewModel(model));
                }

                model.ImageUrl = $"/images/books/{uploadImageResult.ImageName}";
                model.ImageThumbnailUrl = $"/images/books/thumb/{uploadImageResult.ImageName}";
            }


            var CreateBookDTO = _mapper.Map<CreateBookDTO>(model);


            // Call service
            var result = await _bookService.CreateAsync(CreateBookDTO);

            if (result.HasErrorMessage)
            {
                TempData["Error"] = result.ErrorMessage;
                return View("Form", await PopulateViewModel(model));
            }

            // Save image to disk
            if (model.Image != null)
            {
                await SaveImageToDisk(model.Image, model.ImageUrl!);
                await SaveThumbImageToDisk(model.Image, model.ImageThumbnailUrl!);
            }

            TempData["Success"] = "Book created successfully";
            return RedirectToAction(nameof(Details), new { id = result.Result!.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _bookService.GetByIdWithAuthorAndCategoriesAsync(id);
            if (result.HasErrorMessage || result.Result == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Book not found";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<BookFormViewModel>(result.Result);
            return View("Form", await PopulateViewModel(viewModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", await PopulateViewModel(model));


            var book = await _bookService.GetByIdAsync(model.Id);
            if (book != null)
            {
                if (book.HasErrorMessage || book.Result == null)
                {
                    TempData["Error"] = book.ErrorMessage ?? "Book not found";
                    return RedirectToAction(nameof(Index));
                }
            }
            var oldImgUrl = book!.Result!.ImageUrl;
            var oldThumbUrl = book!.Result!.ImageThumbnailUrl;

            var updateBookDto = _mapper.Map<UpdateBookDTO>(model);

            if (model.Image != null)
            {
                var uploadImageResult = await HandleImageUpload(model.Image);
                if (!uploadImageResult.Success)
                {
                    ModelState.AddModelError(nameof(model.Image), uploadImageResult.Error!);
                    var viewModel = await PopulateViewModel(model);
                    return View("Form", viewModel);
                }

                updateBookDto.ImageUrl = $"/images/books/{uploadImageResult.ImageName}";
                updateBookDto.ImageThumbnailUrl = $"/images/books/thumb/{uploadImageResult.ImageName}";
            }
            else if (model.Image is null)
            {
                updateBookDto.ImageUrl = oldImgUrl;
                updateBookDto.ImageThumbnailUrl = oldThumbUrl;
            }

            // Call service
            var result = await _bookService.UpdateAsync(updateBookDto);

            if (result.HasErrorMessage)
            {
                TempData["Error"] = result.ErrorMessage;
                var viewModel = await PopulateViewModel(model);
                return View("Form", viewModel);
            }

            //// Save new image if uploaded
            if (model.Image != null)
            {
                await SaveImageToDisk(model.Image, result.Result!.ImageUrl!);
                await SaveThumbImageToDisk(model.Image, result.Result!.ImageThumbnailUrl!);
                if (!string.IsNullOrEmpty(oldImgUrl))
                {
                    DeleteOldImageFromDisk(oldImgUrl);
                    DeleteOldImageFromDisk(oldThumbUrl);
                }
            }

            TempData["Success"] = "Book updated successfully";
            return RedirectToAction(nameof(Details), new { id = result.Result!.Id });
        }

        public async Task<IActionResult> AllowItem(BookFormViewModel model)
        {
            return Json(await _bookService.IsAllowed(model.Id, model.Title, model.AuthorId));
        }

        private async Task<BookFormViewModel> PopulateViewModel(BookFormViewModel? model = null)
        {
            BookFormViewModel viewModel = model ?? new BookFormViewModel();

            var authorsResult = await _bookService.GetActiveAuthorsForDropdownAsync();
            var categoriesResult = await _bookService.GetActiveCategoriesForDropdownAsync();

            if (authorsResult.Result != null)
                viewModel.Authors = _mapper.Map<IEnumerable<SelectListItem>>(authorsResult.Result);

            if (categoriesResult.Result != null)
                viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(categoriesResult.Result);

            return viewModel;
        }
        private async Task<(bool Success, string? ImageName, string? Error)> HandleImageUpload(IFormFile image)
        {
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

            if (!_allowedExtensions.Contains(extension))
            {
                return (false, null, Errors.NotAllowedExtension);
            }

            if (image.Length > _maxAllowedSize)
            {
                return (false, null, Errors.MaxSize);
            }

            var imageName = $"{Guid.NewGuid()}{extension}";
            return (true, imageName, null);
        }
        private async Task SaveImageToDisk(IFormFile image, string imageUrl)
        {
            var path = $"{_webHostEnvironment.WebRootPath}/{imageUrl}";

            using var stream = System.IO.File.Create(path);
            await image.CopyToAsync(stream);
            stream.Dispose();
        }
        private async Task SaveThumbImageToDisk(IFormFile image, string imageUrl)
        {
            using var thumbImage = Image.Load(image.OpenReadStream());
            var ratio = (float)thumbImage.Width / 200;
            var height = (int)(thumbImage.Height / ratio);
            thumbImage.Mutate(i => i.Resize(200, height));
            thumbImage.Save($"{_webHostEnvironment.WebRootPath}/{imageUrl}");
        }
        private void DeleteOldImageFromDisk(string? imageUrl)
        {
            var oldImagePath = $"{_webHostEnvironment.WebRootPath}/{imageUrl}";

            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);
        }
    }
}


//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> ToggleStatus(int id)
//{
//    var result = await _bookService.ToggleStatusAsync(id, User.Identity?.Name ?? "System");

//    if (result.HasErrorMessage)
//    {
//        TempData["Error"] = result.ErrorMessage;
//    }
//    else
//    {
//        TempData["Success"] = "Book status updated successfully";
//    }

//    return RedirectToAction(nameof(Index));
//}