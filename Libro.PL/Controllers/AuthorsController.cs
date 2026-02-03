using AutoMapper;
using Libro.BLL.DTOs.Author;
using Libro.PL.ViewModels.Author;
namespace Libro.PL.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly IMapper _mapper;
        public AuthorsController(IAuthorService authorService, IMapper mapper)
        {
            _authorService = authorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _authorService.GetAllAsync();
            if (result.HasErrorMessage)
                return StatusCode((int)result.StatusCode, result.ErrorMessage);

            var vm = _mapper.Map<IEnumerable<AuthorViewModel>>(result.Result);
            return View(vm);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form", new AuthorFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState[nameof(model.Name)]?.Errors.First().ErrorMessage);

            var dto = _mapper.Map<CreateAuthorDTO>(model);
            var result = await _authorService.CreateAsync(dto);

            if (result.HasErrorMessage)
                return StatusCode((int)result.StatusCode, result.ErrorMessage);

            var rowVm = _mapper.Map<AuthorViewModel>(result.Result);
            return PartialView("_AuthorRow", rowVm);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _authorService.GetByIdAsync(id);
            if (result.HasErrorMessage)
                return StatusCode((int)result.StatusCode, result.ErrorMessage);

            var vm = _mapper.Map<AuthorFormViewModel>(result.Result);
            return PartialView("_Form", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState[nameof(model.Name)]?.Errors.First().ErrorMessage);

            var dto = _mapper.Map<UpdateAuthorDTO>(model);
            var result = await _authorService.UpdateAsync(dto);

            if (result.HasErrorMessage)
                return StatusCode((int)result.StatusCode, result.ErrorMessage);

            var rowVm = _mapper.Map<AuthorViewModel>(result.Result);
            return PartialView("_AuthorRow", rowVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _authorService.ToggleStatusAsync(id);
            if (result.HasErrorMessage)
                return StatusCode((int)result.StatusCode, result.ErrorMessage);
            var rowVm = _mapper.Map<AuthorViewModel>(result.Result);
            return Ok(rowVm?.UpdatedOn?.ToString());
        }

        public async Task<IActionResult> AllowItem(AuthorFormViewModel model)
        {
            return Json(await _authorService.IsAllowed(model.Id, model.Name));
        }
    }
}
