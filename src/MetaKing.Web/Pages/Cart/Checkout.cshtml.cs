using MetaKing.Extensions;
using MetaKing.Models;
using MetaKing.Orders;
using MetaKing.Orders.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Local;

namespace MetaKing.Pages.Cart
{
    public class CheckoutModel : PageModel
    {
        private readonly IOrdersAppService _ordersAppService;
        private readonly ILocalEventBus _localEventBus;

        public CheckoutModel(IOrdersAppService ordersAppService, ILocalEventBus localEventBus)
        {
            _ordersAppService = ordersAppService;
            _localEventBus = localEventBus;
            Order = new CreateOrderDto();
        }

        [BindProperty]
        public CreateOrderDto Order { get; set; }

        public List<CartItem> CartItems { get; set; } = new();
        public double SubTotal { get; set; }
        public double ShippingFee { get; set; } = 50000;
        public double Total { get; set; }
        public bool? CreateStatus { get; set; }

        public IActionResult OnGet()
        {
            LoadCart();

            if (!CartItems.Any())
                return Redirect("/cart");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            LoadCart();

            if (string.IsNullOrWhiteSpace(Order.CustomerName) ||
                string.IsNullOrWhiteSpace(Order.CustomerPhoneNumber) ||
                string.IsNullOrWhiteSpace(Order.CustomerAddress) ||
                string.IsNullOrWhiteSpace(Order.Province) ||
                string.IsNullOrWhiteSpace(Order.Commune))
            {
                CreateStatus = false;
                return Page();
            }

            if (!ModelState.IsValid || !CartItems.Any())
            {
                CreateStatus = false;

                SubTotal = 0;
                ShippingFee = 0;
                Total = 0;
                return Page();
            }

            Order.Items = CartItems.Select(item => new OrderItemDto
            {
                ProductId = item.Product.Id,
                Price = item.Product.SellPrice,
                Quantity = item.Quantity
            }).ToList();

            Order.CustomerUserId = User.Identity.IsAuthenticated
                ? User.GetUserId()
                : null;

            var order = await _ordersAppService.CreateAsync(Order);

            CreateStatus = order != null;

            //if (CreateStatus == true && Order.CustomerUserId.HasValue)
            //{
            //    var email = User.GetSpecificClaim(ClaimTypes.Email);

            //    await _localEventBus.PublishAsync(new NewOrderCreatedEvent
            //    {
            //        CustomerEmail = email,
            //        Message = "Create order successful."
            //    });
            //}

            if (CreateStatus == true)
            {
                HttpContext.Session.Remove(MetaKingConsts.Cart);

                CartItems = new List<CartItem>();
                SubTotal = 0;
                ShippingFee = 0;
                Total = 0;
            }

            return Page();
        }

        private void LoadCart()
        {
            var cart = HttpContext.Session.GetString(MetaKingConsts.Cart);

            if (!string.IsNullOrEmpty(cart))
            {
                var productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
                CartItems = productCarts?.Values.ToList() ?? new List<CartItem>();
            }
            else
            {
                CartItems = new List<CartItem>();
            }

            SubTotal = CartItems.Sum(x => x.Product.SellPrice * x.Quantity);

            Total = CartItems.Any() ? SubTotal + ShippingFee : 0;
        }
    }
}
