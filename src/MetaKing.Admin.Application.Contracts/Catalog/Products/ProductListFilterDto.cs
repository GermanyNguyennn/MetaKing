using System;
using System.Collections.Generic;
using System.Text;

namespace MetaKing.Admin.Catalog.Products
{
    public class ProductListFilterDto : BaseListFilterDto
    {
        public Guid? CategoryId { get; set; }
    }
}
