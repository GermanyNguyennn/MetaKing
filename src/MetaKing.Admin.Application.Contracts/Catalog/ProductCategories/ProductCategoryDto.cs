using System;
using Volo.Abp.Application.Dtos;

namespace MetaKing.Admin.Catalog.ProductCategories
{
    public class ProductCategoryDto : IEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Slug { get; set; }
        public string CoverPicture { get; set; }
        public bool IsVisibility { get; set; }
        public bool IsActive { get; set; }
        public Guid? ParentId { get; set; }
        public string? ParentName { get; set; }
        public Guid Id { get; set; }
    }
}
