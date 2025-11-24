using MetaKing.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetaKing.Pages.Users
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        private readonly IOrdersAppService _ordersAppService;

        public List<OrderHistoryDto> Orders { get; set; }

        public HistoryModel(IOrdersAppService ordersAppService)
        {
            _ordersAppService = ordersAppService;
        }

        public async Task OnGetAsync()
        {
            // Lấy UserId từ Claims
            var userIdClaim = User?.FindFirst("sub")?.Value
                              ?? User.FindFirst("UserId")?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                Orders = new List<OrderHistoryDto>();
                return;
            }

            Orders = await _ordersAppService.GetOrderHistoryAsync(userId);
        }
    }
}
