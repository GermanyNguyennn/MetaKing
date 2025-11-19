using System;
using System.Collections.Generic;
using System.Text;

namespace MetaKing.Catalog.Products
{
    public class ProductListFilterDto : BaseListFilterDto
    {
        public Guid? CategoryId { get; set; }
        public List<Guid>? CategoryIds { get; set; }

    }
}
