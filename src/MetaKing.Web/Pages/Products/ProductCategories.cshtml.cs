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
        public List<ProductInListDto> Products { get; set; } = new();
        public PagedResult<ProductInListDto> PagedProducts { get; set; }
            = new PagedResult<ProductInListDto>(new List<ProductInListDto>(), 0, 1, 12);

        // Bind từ query string / route
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

            // 1. Lấy cache
            var cacheItem = await _cache.GetOrAddAsync(
                MetaKingPublicConsts.CacheKeys.HomeData,
                LoadHomeDataAsync,
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });

            Categories = cacheItem.Categories;
            Products = cacheItem.Products;

            // 2. Tìm danh mục cha theo slug
            var parentCategory = Categories.FirstOrDefault(c => c.Slug == ParentSlug);
            if (parentCategory == null)
            {
                PagedProducts = new PagedResult<ProductInListDto>(
                    new List<ProductInListDto>(), 0, CurrentPage, PageSize);
                return;
            }

            // 3. Lấy tất cả categoryId con (bao gồm cả cha)
            var categoryIds = GetAllCategoryIds(parentCategory.Id, Categories);

            // 4. Lấy danh sách ProductDto theo categoryIds
            var productsDto = await _productService.GetListByCategoryIdsAsync(categoryIds);

            // 5. Map ProductDto -> ProductInListDto
            var productsInList = productsDto.Select(p => new ProductInListDto
            {
                Id = p.Id,
                Name = p.Name,
                SellPrice = p.SellPrice,
                CategoryId = p.CategoryId,
                CategoryName = p.CategoryName,
                CategorySlug = p.CategorySlug
                // map thêm thuộc tính khác nếu cần
            }).ToList();

            // 6. Phân trang
            var total = productsInList.Count;
            var pagedItems = productsInList
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            PagedProducts = new PagedResult<ProductInListDto>(
                pagedItems, total, CurrentPage, PageSize);
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

        // Lấy tất cả categoryId cha + con
        private List<Guid> GetAllCategoryIds(Guid parentId, List<ProductCategoryInListDto> tree)
        {
            var result = new List<Guid>();

            void Recurse(List<ProductCategoryInListDto> nodes)
            {
                foreach (var node in nodes)
                {
                    if (node.Id == parentId || result.Contains(node.Id))
                    {
                        result.Add(node.Id);
                        if (node.ProductCategory != null)
                            Recurse(node.ProductCategory);
                    }
                    else if (node.ProductCategory != null)
                        Recurse(node.ProductCategory);
                }
            }

            Recurse(tree);
            return result;
        }
    }
}
