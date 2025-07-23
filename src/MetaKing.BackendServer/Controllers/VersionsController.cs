using MetaKing.BackendServer.Data.Entities;
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
    [Route("api/versions")]
    [Authorize]
    [ApiController]
    public class VersionsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public VersionsController(ApplicationDbContext context,
            ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> PostVersion([FromBody] VersionCreateRequest request)
        {
            var version = new VersionModel()
            {
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                Status = request.Status,
                CreatedDate = request.CreatedDate,
            };
            _context.Versions.Add(version);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Versions");
                return CreatedAtAction(nameof(GetById), new { id = version.Id }, result);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse("Create Version failed"));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetVersions()
        {
            var cachedData = await _cacheService.GetAsync<List<VersionViewModel>>("Versions");
            if (cachedData == null)
            {
                var Versions = await _context.Versions.ToListAsync();

                var versionViewModel = Versions.Select(c => CreateVersionViewModel(c)).ToList();
                await _cacheService.SetAsync("Versions", versionViewModel);
                cachedData = versionViewModel;
            }

            return Ok(cachedData);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetVersionsPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Versions.AsQueryable();
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

            var data = items.Select(c => CreateVersionViewModel(c)).ToList();

            var pagination = new Pagination<VersionViewModel>
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
            var version = await _context.Versions.FindAsync(id);
            if (version == null)
                return NotFound(new ApiNotFoundResponse($"Version with id: {id} is not found"));

            VersionViewModel versionViewModel = CreateVersionViewModel(version);

            return Ok(versionViewModel);
        }

        [HttpPut("{id}")]
        [ApiValidationFilter]
        public async Task<IActionResult> PutVersion(int id, [FromBody] VersionCreateRequest request)
        {
            var version = await _context.Versions.FindAsync(id);
            if (version == null)
                return NotFound(new ApiNotFoundResponse($"Version with id: {id} is not found"));

            version.Name = request.Name;
            version.Description = request.Description;
            version.Slug = request.Slug;
            version.Status = request.Status;
            version.CreatedDate = request.CreatedDate;

            _context.Versions.Update(version);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync("Versions");

                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse("Update Version failed"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVersion(int id)
        {
            var version = await _context.Versions.FindAsync(id);
            if (version == null)
                return NotFound(new ApiNotFoundResponse($"Version with id: {id} is not found"));

            _context.Versions.Remove(version);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Versions");

                VersionViewModel versionViewModel = CreateVersionViewModel(version);
                return Ok(versionViewModel);
            }
            return BadRequest();
        }

        private static VersionViewModel CreateVersionViewModel(VersionModel version)
        {
            return new VersionViewModel()
            {
                Id = version.Id,
                Name = version.Name,
                Description = version.Description,
                Slug = version.Slug,
                Status = version.Status,
                CreatedDate = version.CreatedDate,
            };
        }
    }
}
