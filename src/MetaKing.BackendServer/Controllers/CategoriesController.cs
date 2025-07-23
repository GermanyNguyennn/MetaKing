using Azure.Core;
using MetaKing.BackendServer.Data;
using MetaKing.BackendServer.Helpers;
using MetaKing.BackendServer.Services;
using MetaKing.ViewModels;
using MetaKing.ViewModels.Contents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MetaKing.BackendServer.Controllers
{
    [Route("api/categories")]
    [Authorize]
    [ApiController]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public CategoriesController(ApplicationDbContext context,
            ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> PostCategory([FromBody] CategoryCreateRequest request)
        {
            var category = new CategoryModel()
            {
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                Status = request.Status,
                CreatedDate = request.CreatedDate,
            };
            _context.Categories.Add(category);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Categories");
                return CreatedAtAction(nameof(GetById), new {id = category.Id}, request);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse("Create category failed"));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var cachedData = await _cacheService.GetAsync<List<CategoryViewModel>>("Categories");
            if (cachedData == null)
            {
                var categorys = await _context.Categories.ToListAsync();

                var categoryViewModel = categorys.Select(c => CreateCategoryViewModel(c)).ToList();
                await _cacheService.SetAsync("Categories", categoryViewModel);
                cachedData = categoryViewModel;
            }

            return Ok(cachedData);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetCategoriesPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Categories.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Name.Contains(filter));
            }
            var total = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = items.Select(c => CreateCategoryViewModel(c)).ToList();

            var pagination = new Pagination<CategoryViewModel>
            {
                Items = data,
                Total = total,
            };
            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new ApiNotFoundResponse($"Category with id: {id} is not found"));

            CategoryViewModel categoryViewModel = CreateCategoryViewModel(category);

            return Ok(categoryViewModel);
        }

        [HttpPut("{id}")]
        [ApiValidationFilter]
        public async Task<IActionResult> PutCategory(int id, [FromBody] CategoryCreateRequest request)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new ApiNotFoundResponse($"Category with id: {id} is not found"));

            category.Name = request.Name;
            category.Description = request.Description;
            category.Slug = request.Slug;
            category.Status = request.Status;
            category.CreatedDate = request.CreatedDate;

            _context.Categories.Update(category);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync("Categories");

                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse("Update category failed"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new ApiNotFoundResponse($"Category with id: {id} is not found"));

            _context.Categories.Remove(category);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Categories");

                CategoryViewModel categoryViewModel = CreateCategoryViewModel(category);
                return Ok(categoryViewModel);
            }
            return BadRequest();
        }

        private static CategoryViewModel CreateCategoryViewModel(CategoryModel category)
        {
            return new CategoryViewModel()
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug,
                Status = category.Status,
                CreatedDate = category.CreatedDate,
            };
        }
    }
}
