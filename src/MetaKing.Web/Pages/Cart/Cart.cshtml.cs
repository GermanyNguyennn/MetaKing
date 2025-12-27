using MetaKing.Catalog.Products;
using MetaKing.Extensions;
using MetaKing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MetaKing.Pages.Cart
{
    [Authorize]
    public class CartModel : PageModel
    {
        private readonly IProductsAppService _productsAppService;
        public CartModel(IProductsAppService productsAppService)
        {
            _productsAppService = productsAppService;
        }

        [BindProperty]
        public List<CartItem> CartItems { get; set; } = new();

        public double SubTotal { get; set; }
        public double ShippingFee { get; set; } = 50000;
        public double Total { get; set; }

        // --- Load giỏ hàng
        public void LoadCart()
        {
            var cartSession = HttpContext.Session.GetString(MetaKingConsts.Cart);
            if (!string.IsNullOrEmpty(cartSession))
            {
                var productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cartSession);
                CartItems = productCarts?.Values.ToList() ?? new List<CartItem>();
            }

            if (CartItems == null || !CartItems.Any())
            {
                SubTotal = 0;
                ShippingFee = 0;
                Total = 0;
                return;
            }

            SubTotal = CartItems.Sum(x => x.Product.SellPrice * x.Quantity);
            Total = SubTotal + ShippingFee;
        }

        // --- Trang giỏ hàng
        public void OnGet()
        {
            LoadCart();
        }

        // --- Thêm sản phẩm vào giỏ
        public async Task<IActionResult> OnGetAdd(string id, int quantity = 1)
        {
            var cartSession = HttpContext.Session.GetString(MetaKingConsts.Cart);
            var productCarts = string.IsNullOrEmpty(cartSession)
                ? new Dictionary<string, CartItem>()
                : JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cartSession);

            var product = await _productsAppService.GetAsync(Guid.Parse(id));
            if (productCarts.ContainsKey(id))
            {
                productCarts[id].Quantity += quantity;
            }
            else
            {
                productCarts[id] = new CartItem
                {
                    Product = product,
                    Quantity = quantity
                };
            }

            HttpContext.Session.SetString(MetaKingConsts.Cart, JsonSerializer.Serialize(productCarts));

            LoadCart();
            return Page();
        }

        // --- Xóa sản phẩm
        public IActionResult OnGetRemove(string id)
        {
            var cartSession = HttpContext.Session.GetString(MetaKingConsts.Cart);
            if (!string.IsNullOrEmpty(cartSession))
            {
                var productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cartSession);
                if (productCarts.ContainsKey(id))
                {
                    productCarts.Remove(id);
                    HttpContext.Session.SetString(MetaKingConsts.Cart, JsonSerializer.Serialize(productCarts));
                }
            }
            return RedirectToPage();
        }

        // --- Cập nhật số lượng sản phẩm
        public IActionResult OnPostUpdate()
        {
            var cartSession = HttpContext.Session.GetString(MetaKingConsts.Cart);
            var productCarts = string.IsNullOrEmpty(cartSession)
                ? new Dictionary<string, CartItem>()
                : JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cartSession);

            if (CartItems != null && CartItems.Any())
            {
                foreach (var item in CartItems)
                {
                    if (productCarts.ContainsKey(item.Product.Id.ToString()))
                    {
                        productCarts[item.Product.Id.ToString()].Quantity = item.Quantity;

                    }
                }
            }
            HttpContext.Session.SetString(MetaKingConsts.Cart, JsonSerializer.Serialize(productCarts));

            return RedirectToPage();

        }
    }
}
