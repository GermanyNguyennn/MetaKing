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
    public class VendorViewComponent : ViewComponent
    {
        private readonly IDistributedCache<VendorCacheItem> _cache;
        private readonly IManufacturersAppService _manufacturersAppService;
        public List<ManufacturerInListDto> Manufacturers { get; set; } = new List<ManufacturerInListDto>();

        public VendorViewComponent(IManufacturersAppService manufacturersAppService, IDistributedCache<VendorCacheItem> cache)
        {
            _cache = cache;
            _manufacturersAppService = manufacturersAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cacheItem = await _cache.GetOrAddAsync(MetaKingPublicConsts.CacheKeys.VendorData, async () =>
            {          
                var allManufacturers = await _manufacturersAppService.GetListAllAsync();

                return new VendorCacheItem
                {
                    Manufacturers = allManufacturers,
                };
            },
            () => new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
            });

            cacheItem.Manufacturers ??= new List<ManufacturerInListDto>();

            return View(cacheItem);
        }
    }
}
