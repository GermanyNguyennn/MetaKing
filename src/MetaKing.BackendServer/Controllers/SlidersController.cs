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
    [Route("api/sliders")]
    [Authorize]
    [ApiController]
    public class SlidersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public SlidersController(ApplicationDbContext context,
            ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> PostSlider([FromBody] SliderCreateRequest request)
        {
            var slider = new SliderModel()
            {
                Name = request.Name,
                Image = request.Image,
                Description = request.Description,
                Status = request.Status,
                CreatedDate = request.CreatedDate,
            };
            _context.Sliders.Add(slider);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Sliders");
                return CreatedAtAction(nameof(GetById), new { id = slider.Id }, request);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse("Create Slider failed"));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetSliders()
        {
            var cachedData = await _cacheService.GetAsync<List<SliderViewModel>>("Sliders");
            if (cachedData == null)
            {
                var sliders = await _context.Sliders.ToListAsync();

                var sliderViewModel = sliders.Select(c => CreateSliderViewModel(c)).ToList();
                await _cacheService.SetAsync("Sliders", sliderViewModel);
                cachedData = sliderViewModel;
            }

            return Ok(cachedData);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetSlidersPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Sliders.AsQueryable();
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

            var data = items.Select(c => CreateSliderViewModel(c)).ToList();

            var pagination = new Pagination<SliderViewModel>
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
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
                return NotFound(new ApiNotFoundResponse($"Slider with id: {id} is not found"));

            SliderViewModel sliderViewModel = CreateSliderViewModel(slider);

            return Ok(sliderViewModel);
        }

        [HttpPut("{id}")]
        [ApiValidationFilter]
        public async Task<IActionResult> PutSlider(int id, [FromBody] SliderCreateRequest request)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
                return NotFound(new ApiNotFoundResponse($"Slider with id: {id} is not found"));

            slider.Name = request.Name;
            slider.Image = request.Image;
            slider.Description = request.Description;
            slider.Status = request.Status;
            slider.CreatedDate = request.CreatedDate;

            _context.Sliders.Update(slider);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync("Sliders");

                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse("Update Slider failed"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlider(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
                return NotFound(new ApiNotFoundResponse($"Slider with id: {id} is not found"));

            _context.Sliders.Remove(slider);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Sliders");

                SliderViewModel sliderViewModel = CreateSliderViewModel(slider);
                return Ok(sliderViewModel);
            }
            return BadRequest();
        }

        private static SliderViewModel CreateSliderViewModel(SliderModel slider)
        {
            return new SliderViewModel()
            {
                Id = slider.Id,
                Name = slider.Name,
                Image = slider.Image,
                Description = slider.Description,
                Status = slider.Status,
                CreatedDate = slider.CreatedDate,
            };
        }
    }
}
