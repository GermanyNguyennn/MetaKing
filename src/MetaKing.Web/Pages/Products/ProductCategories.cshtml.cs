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
    public class ProductCategoriesModel : PageModel
    {
        private readonly IDistributedCache<HomeCacheItem> _cache;
        private readonly IProductCategoriesAppService _categoryService;
        private readonly IProductsAppService _productService;

        public List<ProductCategoryInListDto> Categories { get; set; } = new();
        public PagedResult<ProductInListDto> PagedProducts { get; set; }
            = new PagedResult<ProductInListDto>(new List<ProductInListDto>(), 0, 1, 12);

        public string ParentSlug { get; set; } = null!;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        public ProductCategoriesModel(
            IProductCategoriesAppService categoryService,
            IProductsAppService productService,
            IDistributedCache<HomeCacheItem> cache)
        {
            _categoryService = categoryService;
            _productService = productService;
            _cache = cache;
        }

        public async Task OnGetAsync(string parentSlug, int currentPage = 1)
        {
            ParentSlug = parentSlug;
            CurrentPage = currentPage;

            var cacheItem = await _cache.GetOrAddAsync(
                MetaKingPublicConsts.CacheKeys.HomeData,
                LoadHomeDataAsync,
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });

            Categories = cacheItem.Categories;

            // 1. tìm danh mục theo slug
            var parentCategory = FindCategoryBySlug(ParentSlug, Categories);

            if (parentCategory == null)
            {
                PagedProducts = new PagedResult<ProductInListDto>(
                    new List<ProductInListDto>(), 0, CurrentPage, PageSize);
                return;
            }

            // 2. lấy toàn bộ categoryId cha + con
            var categoryIds = await _categoryService.GetAllChildrenIdsAsync(parentCategory.Id);

            // 3. lấy sản phẩm thuộc toàn bộ category đó
            var productList = await _productService.GetListByCategoryIdsAsync(categoryIds);

            // 4. phân trang
            var total = productList.Count;

            var pageItems = productList
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            PagedProducts = new PagedResult<ProductInListDto>(
                pageItems, total, CurrentPage, PageSize);
        }

        private ProductCategoryInListDto? FindCategoryBySlug(string slug, List<ProductCategoryInListDto> nodes)
        {
            foreach (var cat in nodes)
            {
                if (cat.Slug == slug)
                    return cat;

                if (cat.ProductCategory != null)
                {
                    var found = FindCategoryBySlug(slug, cat.ProductCategory);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }

        private async Task<HomeCacheItem> LoadHomeDataAsync()
        {
            var categories = await _categoryService.GetListAllAsync();

            var lookup = categories.ToLookup(c => c.ParentId);
            foreach (var c in categories)
                c.ProductCategory = lookup[c.Id].ToList();

            var root = lookup[null].ToList();

            return new HomeCacheItem
            {
                Categories = root,
                Products = new List<ProductInListDto>()
            };
        }
    }
}
