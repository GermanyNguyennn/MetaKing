using MetaKing.Catalog.Manufacturers;
using System.Collections.Generic;

namespace MetaKing.Models
{
    public class VendorCacheItem
    {
        public List<ManufacturerInListDto> Manufacturers { set; get; } = new List<ManufacturerInListDto>();

    }
}
