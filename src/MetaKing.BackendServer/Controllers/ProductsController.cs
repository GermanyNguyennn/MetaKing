using MetaKing.BackendServer.Data;
using MetaKing.BackendServer.Data.Entities;
using MetaKing.BackendServer.Helpers;
using MetaKing.BackendServer.Services;
using MetaKing.ViewModels;
using MetaKing.ViewModels.Contents;
using MetaKing.ViewModels.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MetaKing.BackendServer.Controllers
{
    [Route("api/products")]
    [Authorize]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, ICacheService cacheService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _cacheService = cacheService;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostProduct([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string imageName = "";

            if (request.ImageUpload != null)
            {
                var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/products");
                Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageUpload.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await request.ImageUpload.CopyToAsync(stream);

                imageName = fileName;
            }

            var product = new ProductModel
            {
                Name = request.Name,
                Image = imageName,
                Description = request.Description,
                ProductType = request.ProductType,
                Slug = request.Slug,
                BrandId = request.BrandId,
                CategoryId = request.CategoryId,
                CompanyId = request.CompanyId,
                CreatedDate = request.CreatedDate
            };

            _context.Products.Add(product);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Products");
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, request);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse("Create Product failed"));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var cachedData = await _cacheService.GetAsync<List<ProductViewModel>>("Products");
            if (cachedData == null)
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Company)
                    .ToListAsync();

                var productViewModels = products.Select(CreateProductViewModel).ToList();
                await _cacheService.SetAsync("Products", productViewModels);
                cachedData = productViewModels;
            }

            return Ok(cachedData);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllForAdmin()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Company)
                .ToListAsync();

            return Ok(products.Select(CreateProductViewModel));
        }

        [AllowAnonymous]
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId && p.Status == StatusType.Active)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Company)
                .ToListAsync();

            var result = products.Select(CreateProductViewModel).ToList();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("by-brand/{brandId}")]
        public async Task<IActionResult> GetByBrand(int brandId)
        {
            var products = await _context.Products
                .Where(p => p.BrandId == brandId && p.Status == StatusType.Active)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Company)
                .ToListAsync();

            var result = products.Select(CreateProductViewModel).ToList();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("by-company/{companyId}")]
        public async Task<IActionResult> GetByCompany(int companyId)
        {
            var products = await _context.Products
                .Where(p => p.CompanyId == companyId && p.Status == StatusType.Active)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Company)
                .ToListAsync();

            var result = products.Select(CreateProductViewModel).ToList();
            return Ok(result);
        }


        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> Search(string? keyword, int? categoryId, int? brandId, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Company)
                .Where(p => p.Status == StatusType.Active)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.Name.Contains(keyword));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == brandId);

            var total = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            var data = items.Select(CreateProductViewModel).ToList();

            return Ok(new Pagination<ProductViewModel> { Items = data, Total = total });
        }

        [AllowAnonymous]
        [HttpGet("filter")]
        public async Task<IActionResult> GetProductsPaging(string? filter, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Company)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(p => p.Name.Contains(filter));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = items.Select(c => CreateProductViewModel(c)).ToList();

            var pagination = new Pagination<ProductViewModel>
            {
                Items = data,
                Total = total,
            };
            return Ok(pagination);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Company)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound(new ApiNotFoundResponse($"Product with id {id} is not found"));

            ProductViewModel productViewModel = CreateProductViewModel(product);

            return Ok(productViewModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] ProductCreateRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new ApiNotFoundResponse($"Product with id {id} is not found"));

            product.Name = request.Name;
            product.Image = request.Image;
            product.Description = request.Description;
            product.ProductType = request.ProductType;
            product.Slug = request.Slug;
            product.BrandId = request.BrandId;
            product.CategoryId = request.CategoryId;
            product.CompanyId = request.CompanyId;
            product.CreatedDate = request.CreatedDate;

            _context.Products.Update(product);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync("Products");
                return NoContent();
            }

            return BadRequest(new ApiBadRequestResponse("Update product failed"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new ApiNotFoundResponse($"Product with id {id} is not found"));

            _context.Products.Remove(product);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync("Products");

                ProductViewModel productViewModel = CreateProductViewModel(product);
                return Ok(productViewModel);
            }

            return BadRequest();
        }

        private static ProductViewModel CreateProductViewModel(ProductModel product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Image = product.Image,
                Description = product.Description,
                ProductType = product.ProductType,
                Slug = product.Slug,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
                CompanyId = product.CompanyId,
                CreatedDate = product.CreatedDate,

                Category = product.Category != null
                    ? new CategoryViewModel { Id = product.Category.Id, Name = product.Category.Name }
                    : null,

                Brand = product.Brand != null
                    ? new BrandViewModel { Id = product.Brand.Id, Name = product.Brand.Name }
                    : null,

                Company = product.Company != null
                    ? new CompanyViewModel { Id = product.Company.Id, Name = product.Company.Name }
                    : null
            };
        }

        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetDetailByProductType(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            return product.ProductType switch
            {
                ProductType.Phone => Ok(await _context.ProductDetailPhones.FirstOrDefaultAsync(p => p.ProductId == id)),
                ProductType.Laptop => Ok(await _context.ProductDetailLaptops.FirstOrDefaultAsync(p => p.ProductId == id)),
                ProductType.Watch => Ok(await _context.ProductDetailWatches.FirstOrDefaultAsync(p => p.ProductId == id)),
                _ => BadRequest("Unsupported product type.")
            };
        }

        [HttpPost("{productId}/detail")]
        [Consumes("application/json")]
        public async Task<IActionResult> PostProductDetail(int productId, [FromBody] object detail)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound(new ApiNotFoundResponse($"Product with id: {productId} is not found"));

            switch (product.ProductType)
            {
                case ProductType.Phone:
                    var phoneDetail = System.Text.Json.JsonSerializer.Deserialize<ProductDetailPhoneModel>(detail.ToString()!);
                    phoneDetail!.ProductId = productId;
                    _context.ProductDetailPhones.Add(phoneDetail);
                    break;

                case ProductType.Laptop:
                    var laptopDetail = System.Text.Json.JsonSerializer.Deserialize<ProductDetailLaptopModel>(detail.ToString()!);
                    laptopDetail!.ProductId = productId;
                    _context.ProductDetailLaptops.Add(laptopDetail);
                    break;

                case ProductType.Watch:
                    var tabletDetail = System.Text.Json.JsonSerializer.Deserialize<ProductDetailWatchModel>(detail.ToString()!);
                    tabletDetail!.ProductId = productId;
                    _context.ProductDetailWatches.Add(tabletDetail);
                    break;

                default:
                    return BadRequest("Unsupported product type.");
            }

            var result = await _context.SaveChangesAsync();
            return result > 0
                ? Ok(new ApiSuccessResponse("Detail added successfully."))
                : BadRequest(new ApiBadRequestResponse("Failed to add detail."));
        }

        [HttpPut("{productId}/detail")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutProductDetail(int productId, [FromBody] object detail)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound(new ApiNotFoundResponse($"Product with id: {productId} is not found"));

            switch (product.ProductType)
            {
                case ProductType.Phone:
                    var phoneInput = System.Text.Json.JsonSerializer.Deserialize<ProductDetailPhoneModel>(detail.ToString()!);
                    var existingPhone = await _context.ProductDetailPhones.FirstOrDefaultAsync(x => x.ProductId == productId);
                    if (existingPhone == null) return NotFound("Phone detail not found");

                    _context.Entry(existingPhone).CurrentValues.SetValues(phoneInput!);
                    break;

                case ProductType.Laptop:
                    var laptopInput = System.Text.Json.JsonSerializer.Deserialize<ProductDetailLaptopModel>(detail.ToString()!);
                    var existingLaptop = await _context.ProductDetailLaptops.FirstOrDefaultAsync(x => x.ProductId == productId);
                    if (existingLaptop == null) return NotFound("Laptop detail not found");

                    _context.Entry(existingLaptop).CurrentValues.SetValues(laptopInput!);
                    break;

                case ProductType.Watch:
                    var tabletInput = System.Text.Json.JsonSerializer.Deserialize<ProductDetailWatchModel>(detail.ToString()!);
                    var existingTablet = await _context.ProductDetailWatches.FirstOrDefaultAsync(x => x.ProductId == productId);
                    if (existingTablet == null) return NotFound("Tablet detail not found");

                    _context.Entry(existingTablet).CurrentValues.SetValues(tabletInput!);
                    break;

                default:
                    return BadRequest("Unsupported product type.");
            }

            var result = await _context.SaveChangesAsync();
            await _cacheService.RemoveAsync("Products");
            return result > 0
                ? Ok(new ApiSuccessResponse("Detail updated successfully."))
                : BadRequest(new ApiBadRequestResponse("Failed to update detail."));

        }

        [HttpPost("{id}/phone-detail")]
        public async Task<IActionResult> AddPhoneDetail(int id, [FromBody] ProductDetailPhoneModel model)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.ProductType != ProductType.Phone)
                return NotFound(new ApiNotFoundResponse("Invalid product or not a phone"));

            model.ProductId = id;
            model.BrandId = product.BrandId;
            model.CategoryId = product.CategoryId;
            model.CompanyId = product.CompanyId;

            _context.ProductDetailPhones.Add(model);
            await _context.SaveChangesAsync();
            await _cacheService.RemoveAsync("Products");
            return Ok(model);
        }

        [HttpPut("{id}/phone-detail")]
        public async Task<IActionResult> UpdatePhoneDetail(int id, [FromBody] ProductDetailPhoneModel model)
        {
            var detail = await _context.ProductDetailPhones.FirstOrDefaultAsync(x => x.ProductId == id);
            if (detail == null) return NotFound();

            detail.ScreenSize = model.ScreenSize;
            detail.ScreenTechnology = model.ScreenTechnology;
            detail.RearCamera = model.RearCamera;
            detail.FrontCamera = model.FrontCamera;
            detail.ChipSet = model.ChipSet;
            detail.NFC = model.NFC;
            detail.RAMCapacity = model.RAMCapacity;
            detail.InternalStorage = model.InternalStorage;
            detail.SimCard = model.SimCard;
            detail.OperatingSystem = model.OperatingSystem;
            detail.DisplayResolution = model.DisplayResolution;
            detail.DisplayFeatures = model.DisplayFeatures;
            detail.CPUType = model.CPUType;

            _context.ProductDetailPhones.Update(detail);
            await _context.SaveChangesAsync();
            await _cacheService.RemoveAsync("Products");
            return Ok(detail);
        }

        [HttpDelete("{id}/phone-detail")]
        public async Task<IActionResult> DeletePhoneDetail(int id)
        {
            var detail = await _context.ProductDetailPhones.FirstOrDefaultAsync(x => x.ProductId == id);
            if (detail == null) return NotFound();

            _context.ProductDetailPhones.Remove(detail);
            await _context.SaveChangesAsync();
            await _cacheService.RemoveAsync("Products");
            return Ok();
        }

        [HttpPost("{id}/laptop-detail")]
        public async Task<IActionResult> AddLaptopDetail(int id, [FromBody] ProductDetailLaptopModel model)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.ProductType != ProductType.Laptop)
                return NotFound(new ApiNotFoundResponse("Invalid product or not a laptop"));

            model.ProductId = id;
            model.BrandId = product.BrandId;
            model.CategoryId = product.CategoryId;
            model.CompanyId = product.CompanyId;

            _context.ProductDetailLaptops.Add(model);
            await _context.SaveChangesAsync();
            await _cacheService.RemoveAsync("Products");
            return Ok(model);
        }

        [HttpPut("{id}/laptop-detail")]
        public async Task<IActionResult> UpdateLaptopDetail(int id, [FromBody] ProductDetailLaptopModel model)
        {
            var detail = await _context.ProductDetailLaptops.FirstOrDefaultAsync(x => x.ProductId == id);
            if (detail == null) return NotFound();

            detail.GraphicsCardType = model.GraphicsCardType;
            detail.RAMCapacity = model.RAMCapacity;
            detail.RAMType = model.RAMType;
            detail.NumberOfRAMSlots = model.NumberOfRAMSlots;
            detail.HardDrive = model.HardDrive;
            detail.ScreenSize = model.ScreenSize;
            detail.ScreenTechnology = model.ScreenTechnology;
            detail.Battery = model.Battery;
            detail.OperatingSystem = model.OperatingSystem;
            detail.ScreenResolution = model.ScreenResolution;
            detail.CPUType = model.CPUType;
            detail.Interface = model.Interface;

            _context.ProductDetailLaptops.Update(detail);
            await _context.SaveChangesAsync();
            await _cacheService.RemoveAsync("Products");
            return Ok(detail);
        }

        [HttpDelete("{id}/laptop-detail")]
        public async Task<IActionResult> DeleteLaptopDetail(int id)
        {
            var detail = await _context.ProductDetailLaptops.FirstOrDefaultAsync(x => x.ProductId == id);
            if (detail == null) return NotFound();

            _context.ProductDetailLaptops.Remove(detail);
            await _context.SaveChangesAsync();
            await _cacheService.RemoveAsync("Products");
            return Ok();
        }

        [HttpPost("{id}/watch-detail")]
        public async Task<IActionResult> AddWatchDetail(int id, [FromBody] ProductDetailWatchModel model)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.ProductType != ProductType.Watch)
                return NotFound(new ApiNotFoundResponse("Invalid product or not a watch"));

            model.ProductId = id;
            model.BrandId = product.BrandId;
            model.CategoryId = product.CategoryId;
            model.CompanyId = product.CompanyId;

            _context.ProductDetailWatches.Add(model);
            await _context.SaveChangesAsync();
            await _cacheService.RemoveAsync("Products");
            return Ok(model);
        }

        [HttpPut("{id}/watch-detail")]
        public async Task<IActionResult> UpdateWatchDetail(int id, [FromBody] ProductDetailWatchModel model)
        {
            var detail = await _context.ProductDetailWatches.FirstOrDefaultAsync(x => x.ProductId == id);
            if (detail == null) return NotFound();

            detail.ScreenTechnology = model.ScreenTechnology;
            detail.FaceDiameter = model.FaceDiameter;
            detail.SuitableWristSize = model.SuitableWristSize;
            detail.ListenCall = model.ListenCall;
            detail.HealthBenefits = model.HealthBenefits;
            detail.Compatible = model.Compatible;
            detail.BatteryLife = model.BatteryLife;

            _context.ProductDetailWatches.Update(detail);
            await _context.SaveChangesAsync();
            await _cacheService.RemoveAsync("Products");
            return Ok(detail);
        }

        [HttpDelete("{id}/watch-detail")]
        public async Task<IActionResult> DeleteWatchDetail(int id)
        {
            var detail = await _context.ProductDetailWatches.FirstOrDefaultAsync(x => x.ProductId == id);
            if (detail == null) return NotFound();

            _context.ProductDetailWatches.Remove(detail);
            await _context.SaveChangesAsync();
            await _cacheService.RemoveAsync("Products");
            return Ok();
        }

    }
}
