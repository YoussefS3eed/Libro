using Libro.BLL.ModelVM.Author;

namespace Libro.PL.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            if (authors.HasErrorMessage)
            {
                return StatusCode((int)authors.StatusCode, authors.ErrorMessage);
            }

            return View(authors.Result);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorFormVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState[nameof(model.Name)]?.Errors[0].ErrorMessage);
            }

            var author = await _authorService.CreateAuthorAsync(model);
            if (author.HasErrorMessage)
            {
                return StatusCode((int)author.StatusCode, author.ErrorMessage);
            }

            return PartialView("_AuthorRow", author?.Result);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author.HasErrorMessage)
            {
                return StatusCode((int)author.StatusCode, author.ErrorMessage);
            }

            return PartialView("_Form", author.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AuthorFormVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState[nameof(model.Name)]?.Errors[0].ErrorMessage);
            }

            var author = await _authorService.UpdateAuthorAsync(model);
            if (author.HasErrorMessage)
            {
                return StatusCode((int)author.StatusCode, author.ErrorMessage);
            }

            return PartialView("_AuthorRow", author.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var author = await _authorService.ToggleStatusAuthorAsync(id);
            if (author.HasErrorMessage)
            {
                return StatusCode((int)author.StatusCode, author.ErrorMessage);
            }

            return Ok(author.Result!.UpdatedOn?.ToString());
        }
    }
}
