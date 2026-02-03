using Libro.BLL.DTOs.Category;
using System.Net;

namespace Libro.BLL.Service.Implementation
{
    public class CategoryService : ICategoryService, IUniqueNameValidator
    {
        private readonly ICategoryRepo _categoryRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(ICategoryRepo categoryRepo, IMapper mapper, ILogger<CategoryService> logger)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Response<CategoryDto>> CreateAsync(CreateCategoryDTO dto)
        {
            try
            {
                if (dto == null)
                    return new(null, "Invalid data.", true, HttpStatusCode.BadRequest);

                if (await NameExistsAsync(dto.Name))
                    return new(null, "Category name already exists.", true, HttpStatusCode.Conflict);

                var category = _mapper.Map<Category>(dto);
                var result = await _categoryRepo.AddAsync(category);

                if (result == null)
                    return new(null, "Failed to create category in database.", true, HttpStatusCode.InternalServerError);

                return new(_mapper.Map<CategoryDto>(result), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category {Name}", dto?.Name);
                return new(null, "Unexpected error.", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<CategoryDto>> UpdateAsync(UpdateCategoryDTO dto)
        {
            try
            {
                if (dto == null)
                    return new(null, "Invalid data.", true, HttpStatusCode.BadRequest);

                var existingCategory = await _categoryRepo.GetByIdAsync(dto.Id);
                if (existingCategory == null)
                    return new(null, "Category not found.", true, HttpStatusCode.NotFound);

                // Check if name changed and validate uniqueness
                if (existingCategory.Name != dto.Name && await NameExistsAsync(dto.Name))
                    return new(null, "Category name already exists.", true, HttpStatusCode.Conflict);

                var category = _mapper.Map<Category>(dto);
                var result = await _categoryRepo.UpdateAsync(category);
                if (result == null)
                    return new(null, "Database error.", true, HttpStatusCode.BadRequest);

                return new(_mapper.Map<CategoryDto>(result), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {Name}", dto?.Name);
                return new(null, "Unexpected error.", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<CategoryDto>> ToggleStatusAsync(int categoryId)
        {
            try
            {
                var category = await _categoryRepo.GetByIdAsync(categoryId);
                if (category == null)
                    return new(null, "Category not found.", true, HttpStatusCode.NotFound);

                var result = await _categoryRepo.ToggleStatusAsync(category.Id);
                if (result == null)
                    return new(null, "Database error.", true, HttpStatusCode.BadRequest);

                return new(_mapper.Map<CategoryDto>(result), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status for category {Id}", categoryId);
                return new(null, "Unexpected error.", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<CategoryDto>> GetByIdAsync(int categoryId)
        {
            try
            {
                var category = await _categoryRepo.GetByIdAsync(categoryId);
                if (category == null)
                    return new(null, "Category not found.", true, HttpStatusCode.NotFound);

                return new(_mapper.Map<CategoryDto>(category), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category by id {Id}", categoryId);
                return new(null, "Could not load category.", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<IEnumerable<CategoryDto>>> GetAllAsync()
        {
            try
            {
                var categories = await _categoryRepo.GetAllAsync();
                return new(_mapper.Map<IEnumerable<CategoryDto>>(categories), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active categories");
                return new(null, "Could not load categories.", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Response<IEnumerable<CategoryDto>>> GetAllNotActiveAsync()
        {
            try
            {
                var categories = await _categoryRepo.GetAllAsync(c => c.IsDeleted);
                return new(_mapper.Map<IEnumerable<CategoryDto>>(categories), null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching inactive categories");
                return new(null, "Could not load categories.", true, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<bool> NameExistsAsync(string name) =>
            await _categoryRepo.AnyAsync(c => c.Name == name);
        public async Task<bool> IsAllowed(int Id, string name)
        {
            var category = await _categoryRepo.GetSingleOrDefaultAsync(c => c.Name == name);
            return category is null || category.Id.Equals(Id);
        }
        private string GetCurrentUser()
        {
            // TODO: Implement this to return the current user for categories
            // يجب تنفيذ هذا ليعيد المستخدم الحالي
            // مثال: يمكن استخدام IHttpContextAccessor
            return "System"; // مؤقت
        }
    }
}