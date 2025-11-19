using System.Collections.Generic;
using MetaKing.Catalog.ProductCategories;
using MetaKing.Catalog.Products;

namespace MetaKing.Models
{
    public class HomeCacheItem
    {
        public List<ProductCategoryInListDto> Categories { set; get; } = new List<ProductCategoryInListDto>();
        public List<ProductInListDto> Products { set; get; } = new List<ProductInListDto>();

    }
}
