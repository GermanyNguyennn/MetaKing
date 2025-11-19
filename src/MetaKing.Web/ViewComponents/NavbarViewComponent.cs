using MetaKing.Catalog.Manufacturers;
using MetaKing.Catalog.ProductCategories;
using MetaKing.Catalog.Products;
using MetaKing.Manufacturers;
using MetaKing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace MetaKing.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly IDistributedCache<HomeCacheItem> _cache;
        private readonly IProductCategoriesAppService _categoriesAppService;

        public NavbarViewComponent(
            IProductCategoriesAppService categoriesAppService,
            IDistributedCache<HomeCacheItem> cache)
        {
            _categoriesAppService = categoriesAppService;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cacheItem = await _cache.GetOrAddAsync(
                MetaKingPublicConsts.CacheKeys.HomeData,
                GetDataAsync,
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });

            return View(cacheItem);
        }

        private async Task<HomeCacheItem> GetDataAsync()
        {
            var list = await _categoriesAppService.GetListAllAsync();

            var lookup = list.ToLookup(x => x.ParentId);

            foreach (var category in list)
            {
                category.ProductCategory = lookup[category.Id].ToList();
            }

            var roots = lookup[null].ToList();

            return new HomeCacheItem
            {
                Categories = roots
            };
        }
    }
}
