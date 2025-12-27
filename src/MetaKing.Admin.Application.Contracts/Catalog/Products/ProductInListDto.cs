using System;
using MetaKing.Products;
using Volo.Abp.Application.Dtos;

namespace MetaKing.Admin.Catalog.Products
{
    public class ProductInListDto : EntityDto<Guid>
    {
        public Guid ManufacturerId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public ProductType ProductType { get; set; }
        public string Code { get; set; }
        public bool IsVisibility { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
        public string ThumbnailPicture { get; set; }
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }
    }
}
