using AutoMapper;
using Libro.BLL.DTOs.Category;
using Libro.PL.ViewModels.Category;


namespace Libro.PL.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _categoryService.GetAllAsync();
            if (result.HasErrorMessage)
                return StatusCode((int)result.StatusCode, result.ErrorMessage);

            var vm = _mapper.Map<IEnumerable<CategoryViewModel>>(result.Result);
            return View(vm);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form", new CategoryFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState[nameof(model.Name)]?.Errors.First().ErrorMessage);

            var dto = _mapper.Map<CreateCategoryDTO>(model);
            var result = await _categoryService.CreateAsync(dto);

            if (result.HasErrorMessage)
                return StatusCode((int)result.StatusCode, result.ErrorMessage);

            var rowVm = _mapper.Map<CategoryViewModel>(result.Result);
            return PartialView("_CategoryRow", rowVm);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (result.HasErrorMessage)
                return StatusCode((int)result.StatusCode, result.ErrorMessage);

            var vm = _mapper.Map<CategoryFormViewModel>(result.Result);
            return PartialView("_Form", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState[nameof(model.Name)]?.Errors.First().ErrorMessage);

            var dto = _mapper.Map<UpdateCategoryDTO>(model);
            var result = await _categoryService.UpdateAsync(dto);

            if (result.HasErrorMessage)
                return StatusCode((int)result.StatusCode, result.ErrorMessage);

            var rowVm = _mapper.Map<CategoryViewModel>(result.Result);
            return PartialView("_CategoryRow", rowVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _categoryService.ToggleStatusAsync(id);
            if (result.HasErrorMessage)
                return StatusCode((int)result.StatusCode, result.ErrorMessage);

            var rowVm = _mapper.Map<CategoryViewModel>(result.Result);
            return Ok(rowVm?.UpdatedOn?.ToString());
        }

        public async Task<IActionResult> AllowItem(CategoryFormViewModel model)
        {
            return Json(await _categoryService.IsAllowed(model.Id, model.Name));
        }
    }
}