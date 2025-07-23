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
    [Route("api/companies")]
    [Authorize]
    [ApiController]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public CompaniesController(ApplicationDbContext context,
            ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> PostCompany([FromBody] CompanyCreateRequest request)
        {
            var company = new CompanyModel()
            {
                Name = request.Name,
                Description = request.Description,
                Slug = request.Slug,
                Status = request.Status,
                CreatedDate = request.CreatedDate,
            };
            _context.Companies.Add(company);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Companies");
                return CreatedAtAction(nameof(GetById), new { id = company.Id }, request);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse("Create Company failed"));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCompanies()
        {
            var cachedData = await _cacheService.GetAsync<List<CompanyViewModel>>("Companies");
            if (cachedData == null)
            {
                var companys = await _context.Companies.ToListAsync();

                var companyViewModel = companys.Select(c => CreateCompanyViewModel(c)).ToList();
                await _cacheService.SetAsync("Companies", companyViewModel);
                cachedData = companyViewModel;
            }

            return Ok(cachedData);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetCompaniesPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Companies.AsQueryable();
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

            var data = items.Select(c => CreateCompanyViewModel(c)).ToList();

            var pagination = new Pagination<CompanyViewModel>
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
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
                return NotFound(new ApiNotFoundResponse($"Company with id: {id} is not found"));

            CompanyViewModel companyViewModel = CreateCompanyViewModel(company);

            return Ok(companyViewModel);
        }

        [HttpPut("{id}")]
        [ApiValidationFilter]
        public async Task<IActionResult> PutCompany(int id, [FromBody] CompanyCreateRequest request)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
                return NotFound(new ApiNotFoundResponse($"Company with id: {id} is not found"));

            company.Name = request.Name;
            company.Description = request.Description;
            company.Slug = request.Slug;
            company.Status = request.Status;
            company.CreatedDate = request.CreatedDate;

            _context.Companies.Update(company);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync("Companies");

                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse("Update Company failed"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
                return NotFound(new ApiNotFoundResponse($"Company with id: {id} is not found"));

            _context.Companies.Remove(company);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Companies");

                CompanyViewModel companyViewModel = CreateCompanyViewModel(company);
                return Ok(companyViewModel);
            }
            return BadRequest();
        }

        private static CompanyViewModel CreateCompanyViewModel(CompanyModel company)
        {
            return new CompanyViewModel()
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                Slug = company.Slug,
                Status = company.Status,
                CreatedDate = company.CreatedDate,
            };
        }
    }
}
