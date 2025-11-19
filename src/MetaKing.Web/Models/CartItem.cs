using System;
using MetaKing.Catalog.Products;

namespace MetaKing.Models
{
    public class CartItem
    {
        public ProductDto Product { get; set; }
        public int Quantity { get; set; }
    }
}
