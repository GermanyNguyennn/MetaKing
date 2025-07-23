using MetaKing.BackendServer.Data;
using MetaKing.BackendServer.Helpers;
using MetaKing.BackendServer.Services;
using MetaKing.ViewModels.Contents;
using MetaKing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MetaKing.ViewModels.Enum;
using Azure.Core;

namespace MetaKing.BackendServer.Controllers
{
    [Route("api/admin/coupons")]
    [Authorize]
    [ApiController]
    public class CouponsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public CouponsController(ApplicationDbContext context,
            ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        private async Task<IActionResult?> ValidateCouponRequestAsync(CouponCreateRequest request, int? existingId = null)
        {
            if (request.StartDate >= request.EndDate)
            {
                return BadRequest(new ApiBadRequestResponse("Start date must be less than end date."));
            }

            var couponExists = await _context.Coupons
                .AnyAsync(c => c.CouponCode.ToLower() == request.CouponCode.ToLower()
                            && (!existingId.HasValue || c.Id != existingId.Value));

            if (couponExists)
            {
                return Conflict(new ApiConflictResponse("Coupon code already exists."));
            }

            return null;
        }


        [HttpPost]
        public async Task<IActionResult> PostCoupon([FromBody] CouponCreateRequest request)
        {
            var validationResult = await ValidateCouponRequestAsync(request);
            if (validationResult != null)
                return validationResult;

            var coupon = new CouponModel()
            {
                CouponCode = request.CouponCode,
                Description = request.Description,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Quantity = request.Quantity,
                Status = request.Status,
                CreatedDate = request.CreatedDate,
            };
            _context.Coupons.Add(coupon);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Coupons");
                return CreatedAtAction(nameof(GetById), new { id = coupon.Id }, request);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse("Create Coupon failed"));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCoupons()
        {
            var cachedData = await _cacheService.GetAsync<List<CouponViewModel>>("Coupons");
            if (cachedData == null)
            {
                var coupons = await _context.Coupons.ToListAsync();

                var couponViewModel = coupons.Select(c => CreateCouponViewModel(c)).ToList();
                await _cacheService.SetAsync("Coupons", couponViewModel);
                cachedData = couponViewModel;
            }

            return Ok(cachedData);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetCouponsPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Coupons.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.CouponCode.Contains(filter));
            }
            var total = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = items.Select(c => CreateCouponViewModel(c)).ToList();

            var pagination = new Pagination<CouponViewModel>
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
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
                return NotFound(new ApiNotFoundResponse($"Coupon with id: {id} is not found"));

            CouponViewModel couponViewModel = CreateCouponViewModel(coupon);

            return Ok(couponViewModel);
        }

        [HttpPut("{id}")]
        [ApiValidationFilter]
        public async Task<IActionResult> PutCoupon(int id, [FromBody] CouponCreateRequest request)
        {
            var validationResult = await ValidateCouponRequestAsync(request, id);
            if (validationResult != null)
                return validationResult;

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
                return NotFound(new ApiNotFoundResponse($"Coupon with id: {id} is not found"));

            coupon.CouponCode = request.CouponCode;
            coupon.Description = request.Description;
            coupon.DiscountType = request.DiscountType;
            coupon.DiscountValue = request.DiscountValue;
            coupon.StartDate = request.StartDate;
            coupon.EndDate = request.EndDate;
            coupon.Quantity = request.Quantity;
            coupon.Status = request.Status;
            coupon.CreatedDate = request.CreatedDate;

            _context.Coupons.Update(coupon);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync("Coupons");
                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse("Update Coupon failed"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
                return NotFound(new ApiNotFoundResponse($"Coupon with id: {id} is not found"));

            _context.Coupons.Remove(coupon);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Coupons");

                CouponViewModel couponViewModel = CreateCouponViewModel(coupon);
                return Ok(couponViewModel);
            }
            return BadRequest();
        }

        private static CouponViewModel CreateCouponViewModel(CouponModel coupon)
        {
            return new CouponViewModel()
            {
                CouponCode = coupon.CouponCode,
                Description = coupon.Description,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                Quantity = coupon.Quantity,
                Status = coupon.Status,
                CreatedDate = coupon.CreatedDate,
            };
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
                return NotFound(new ApiNotFoundResponse($"Coupon with id: {id} is not found"));

            coupon.Status = coupon.Status == StatusType.Active ? StatusType.Inactive : StatusType.Active;
            await _context.SaveChangesAsync();
            await _cacheService.RemoveAsync("Coupons");

            return Ok(new
            {
                message = "Coupon status updated.",
                newStatus = coupon.Status
            });
        }

    }
}
