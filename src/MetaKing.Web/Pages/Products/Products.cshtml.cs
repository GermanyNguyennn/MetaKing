using MetaKing.Catalog.ProductCategories;
using MetaKing.Catalog.Products;
using MetaKing.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace MetaKing.Pages.Products
{
    public class ProductsModel : PageModel
    {
        private readonly IDistributedCache<HomeCacheItem> _cache;
        private readonly IProductCategoriesAppService _categoryService;
        private readonly IProductsAppService _productService;

        public List<ProductCategoryInListDto> Categories { get; set; } = new();
        public List<ProductInListDto> Products { get; set; } = new();
        public PagedResult<ProductInListDto> PagedProducts { get; set; }

        public ProductsModel(
            IProductCategoriesAppService categoryService,
            IProductsAppService productService,
            IDistributedCache<HomeCacheItem> cache)
        {
            _categoryService = categoryService;
            _productService = productService;
            _cache = cache;
        }

        public async Task OnGetAsync(int currentPage = 1)
        {
            var cacheItem = await _cache.GetOrAddAsync(
                MetaKingPublicConsts.CacheKeys.HomeData,
                LoadHomeDataAsync,
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });

            Categories = cacheItem.Categories;
            Products = cacheItem.Products;

            var input = new ProductListFilterDto
            {
                CurrentPage = currentPage
            };

            PagedProducts = await _productService.GetListFilterAsync(input);
        }

        private async Task<HomeCacheItem> LoadHomeDataAsync()
        {
            var allCategories = await _categoryService.GetListAllAsync();

            var lookup = allCategories.ToLookup(c => c.ParentId);

            foreach (var cat in allCategories)
                cat.ProductCategory = lookup[cat.Id].ToList();

            var rootCategories = lookup[null].ToList();

            var products = await _productService.GetListAllAsync();

            var categoryDict = allCategories.ToDictionary(c => c.Id, c => c);

            foreach (var product in products)
            {
                if (categoryDict.TryGetValue(product.CategoryId, out var cat))
                {
                    product.CategoryName = cat.Name;
                    product.CategorySlug = cat.Slug;
                }
            }

            return new HomeCacheItem
            {
                Categories = rootCategories,
                Products = products
            };
        }
    }
}
