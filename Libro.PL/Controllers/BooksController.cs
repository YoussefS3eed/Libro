using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet.Core;
using Libro.BLL.DTOs.Book;
using Libro.PL.Settings;
using Libro.PL.ViewModels.Book;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Libro.PL.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Cloudinary _cloudinary;

        private readonly List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private const int _maxAllowedSize = 2097152; // 2MB

        public BooksController(IBookService bookService, IMapper mapper,
            IWebHostEnvironment webHostEnvironment, IOptions<CloudinarySettings> cloudinary)
        {
            _bookService = bookService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;

            _cloudinary = new Cloudinary(new Account(cloudinary.Value.Cloud, cloudinary.Value.ApiKey, cloudinary.Value.ApiSecret));
        }

        public async Task<IActionResult> Index()
        {
            return View();
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

            #region using Cloudinary
            var CreateBookDTO = _mapper.Map<CreateBookDTO>(model);
            #endregion

            // Handle image upload
            if (model.Image != null)
            {
                var uploadImageResult = await HandleImageUpload(model.Image);
                if (!uploadImageResult.Success)
                {
                    ModelState.AddModelError(nameof(model.Image), uploadImageResult.Error!);
                    return View("Form", await PopulateViewModel(model));
                }
                #region using SaveInDisk
                //model.ImageUrl = uploadImageResult.ImageUrl;
                #endregion

                #region using Cloudinary
                var result1 = await SaveImageToCloudinary(model.Image, uploadImageResult.ImageUrl!);
                if (result1 != null)
                {
                    CreateBookDTO.ImageUrl = result1.SecureUrl.ToString();
                    CreateBookDTO.ImageThumbnailUrl = GetThumbnailUrl(CreateBookDTO.ImageUrl);
                    CreateBookDTO.ImagePublicId = result1.PublicId;
                }
                #endregion
            }

            #region using SaveInDisk
            //var CreateBookDTO = _mapper.Map<CreateBookDTO>(model);
            #endregion

            // Call service
            var result = await _bookService.CreateAsync(CreateBookDTO);

            if (result.HasErrorMessage)
            {
                TempData["Error"] = result.ErrorMessage;
                return View("Form", await PopulateViewModel(model));
            }
            #region using SaveInDisk
            //if (model.Image != null)
            //    await SaveImageToDisk(model.Image, result.Result?.ImageUrl!);
            #endregion

            TempData["Success"] = "Book created successfully";
            return RedirectToAction(nameof(Index));
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
            var updateBookDto = _mapper.Map<UpdateBookDTO>(model);
            if (model.Image != null)
            {
                var uploadResult = await HandleImageUpload(model.Image);
                if (!uploadResult.Success)
                {
                    ModelState.AddModelError(nameof(model.Image), uploadResult.Error!);
                    var viewModel = await PopulateViewModel(model);
                    return View("Form", viewModel);
                }
                // Delete old image if exists
                if (!string.IsNullOrEmpty(book.Result!.ImageUrl))
                {
                    #region using SaveInDisk
                    //DeleteOldImageFromDisk(book.Result!.ImageUrl);
                    #endregion

                    #region using Cloudinary
                    await DeleteOldImageFromCloudinary(book.Result.ImagePublicId);
                    var result1 = await SaveImageToCloudinary(model.Image, uploadResult.ImageUrl!);
                    if (result1 != null)
                    {
                        updateBookDto.ImageUrl = result1.SecureUrl.ToString();
                        updateBookDto.ImageThumbnailUrl = GetThumbnailUrl(updateBookDto.ImageUrl);
                        updateBookDto.ImagePublicId = result1.PublicId;
                    }
                    #endregion
                }

            }
            else if (model.Image is null && !string.IsNullOrEmpty(book.Result!.ImageUrl))
            {
                updateBookDto.ImageUrl = book.Result!.ImageUrl;
                updateBookDto.ImageThumbnailUrl = book.Result!.ImageThumbnailUrl;
                updateBookDto.ImagePublicId = book.Result!.ImagePublicId;

            }

            #region using SaveInDisk
            //var updateBookDto = _mapper.Map<UpdateBookDTO>(model);
            #endregion

            // Call service
            var result = await _bookService.UpdateAsync(updateBookDto);

            if (result.HasErrorMessage)
            {
                TempData["Error"] = result.ErrorMessage;
                var viewModel = await PopulateViewModel(model);
                return View("Form", viewModel);
            }

            //// Save new image if uploaded
            //if (model.Image != null && !string.IsNullOrEmpty(model.ImageUrl))
            //{
            //    await SaveImageToDisk(model.Image, model.ImageUrl);
            //}

            TempData["Success"] = "Book updated successfully";
            return RedirectToAction(nameof(Index));
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
        private async Task<(bool Success, string? ImageUrl, string? Error)> HandleImageUpload(IFormFile image)
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
        private async Task SaveImageToDisk(IFormFile image, string imageName)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "images/books", imageName);

            using var stream = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(stream);
        }

        private async Task<ImageUploadResult?> SaveImageToCloudinary(IFormFile image, string imageName)
        {
            using var straem = image.OpenReadStream();

            var imageParams = new ImageUploadParams
            {
                File = new FileDescription(imageName, straem),
                UseFilename = true
            };
            return await _cloudinary.UploadAsync(imageParams);
        }
        private void DeleteOldImageFromDisk(string imageUrl)
        {
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/books", imageUrl);

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
        }

        private async Task DeleteOldImageFromCloudinary(string? imagePublicId)
        {
            await _cloudinary.DeleteResourcesAsync(imagePublicId);
        }
        private string GetThumbnailUrl(string url)
        {
            var separator = "image/upload/";
            var urlParts = url.Split(separator);

            var thumbnailUrl = $"{urlParts[0]}{separator}c_thumb,w_200,g_face/{urlParts[1]}";

            return thumbnailUrl;
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