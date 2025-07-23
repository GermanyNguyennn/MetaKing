using MetaKing.BackendServer.Data;
using MetaKing.BackendServer.Helpers;
using MetaKing.BackendServer.Services;
using MetaKing.ViewModels.Contents;
using MetaKing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MetaKing.BackendServer.Data.Entities;

namespace MetaKing.BackendServer.Controllers
{
    [Route("api/colors")]
    [Authorize]
    [ApiController]
    public class ColorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public ColorsController(ApplicationDbContext context,
            ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> PostColor([FromBody] ColorCreateRequest request)
        {
            var color = new ColorModel()
            {
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                Status = request.Status,
                CreatedDate = request.CreatedDate,
            };
            _context.Colors.Add(color);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Colors");
                return CreatedAtAction(nameof(GetById), new { id = color.Id }, request);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse("Create Color failed"));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetColors()
        {
            var cachedData = await _cacheService.GetAsync<List<ColorViewModel>>("Colors");
            if (cachedData == null)
            {
                var Colors = await _context.Colors.ToListAsync();

                var ColorViewModel = Colors.Select(c => CreateColorViewModel(c)).ToList();
                await _cacheService.SetAsync("Colors", ColorViewModel);
                cachedData = ColorViewModel;
            }

            return Ok(cachedData);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetColorsPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Colors.AsQueryable();
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

            var data = items.Select(c => CreateColorViewModel(c)).ToList();

            var pagination = new Pagination<ColorViewModel>
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
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
                return NotFound(new ApiNotFoundResponse($"Color with id: {id} is not found"));

            ColorViewModel colorViewModel = CreateColorViewModel(color);

            return Ok(colorViewModel);
        }

        [HttpPut("{id}")]
        [ApiValidationFilter]
        public async Task<IActionResult> PutColor(int id, [FromBody] ColorCreateRequest request)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
                return NotFound(new ApiNotFoundResponse($"Color with id: {id} is not found"));

            color.Name = request.Name;
            color.Description = request.Description;
            color.Slug = request.Slug;
            color.Status = request.Status;
            color.CreatedDate = request.CreatedDate;

            _context.Colors.Update(color);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync("Colors");

                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse("Update Color failed"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
                return NotFound(new ApiNotFoundResponse($"Color with id: {id} is not found"));

            _context.Colors.Remove(color);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Colors");

                ColorViewModel colorViewModel = CreateColorViewModel(color);
                return Ok(colorViewModel);
            }
            return BadRequest();
        }

        private static ColorViewModel CreateColorViewModel(ColorModel color)
        {
            return new ColorViewModel()
            {
                Id = color.Id,
                Name = color.Name,
                Description = color.Description,
                Slug = color.Slug,
                Status = color.Status,
                CreatedDate = color.CreatedDate,
            };
        }
    }
}