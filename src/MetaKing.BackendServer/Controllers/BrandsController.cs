using MetaKing.BackendServer.Data;
using MetaKing.BackendServer.Helpers;
using MetaKing.BackendServer.Services;
using MetaKing.ViewModels.Contents;
using MetaKing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MetaKing.BackendServer.Controllers
{
    [Route("api/brands")]
    [Authorize]
    [ApiController]
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public BrandsController(ApplicationDbContext context,
            ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> PostBrand([FromBody] BrandCreateRequest request)
        {
            var brand = new BrandModel()
            {
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                Status = request.Status,
                CreatedDate = request.CreatedDate,
            };
            _context.Brands.Add(brand);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Brands");
                return CreatedAtAction(nameof(GetById), new { id = brand.Id }, request);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse("Create Brand failed"));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrands()
        {
            var cachedData = await _cacheService.GetAsync<List<BrandViewModel>>("Brands");
            if (cachedData == null)
            {
                var brands = await _context.Brands.ToListAsync();

                var brandViewModel = brands.Select(c => CreateBrandViewModel(c)).ToList();
                await _cacheService.SetAsync("Brands", brandViewModel);
                cachedData = brandViewModel;
            }

            return Ok(cachedData);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetBrandsPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Brands.AsQueryable();
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

            var data = items.Select(c => CreateBrandViewModel(c)).ToList();

            var pagination = new Pagination<BrandViewModel>
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
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
                return NotFound(new ApiNotFoundResponse($"Brand with id: {id} is not found"));

            BrandViewModel brandViewModel = CreateBrandViewModel(brand);

            return Ok(brandViewModel);
        }

        [HttpPut("{id}")]
        [ApiValidationFilter]
        public async Task<IActionResult> PutBrand(int id, [FromBody] BrandCreateRequest request)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
                return NotFound(new ApiNotFoundResponse($"Brand with id: {id} is not found"));

            brand.Name = request.Name;
            brand.Description = request.Description;
            brand.Slug = request.Slug;
            brand.Status = request.Status;
            brand.CreatedDate = request.CreatedDate;

            _context.Brands.Update(brand);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync("Brands");

                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse("Update Brand failed"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
                return NotFound(new ApiNotFoundResponse($"Brand with id: {id} is not found"));

            _context.Brands.Remove(brand);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Brands");

                BrandViewModel brandViewModel = CreateBrandViewModel(brand);
                return Ok(brandViewModel);
            }
            return BadRequest();
        }

        private static BrandViewModel CreateBrandViewModel(BrandModel brand)
        {
            return new BrandViewModel()
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                Slug = brand.Slug,
                Status = brand.Status,
                CreatedDate = brand.CreatedDate,
            };
        }
    }
}
