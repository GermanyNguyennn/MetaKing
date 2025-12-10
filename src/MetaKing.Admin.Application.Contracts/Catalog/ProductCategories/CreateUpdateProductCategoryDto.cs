using System;

namespace MetaKing.Admin.Catalog.ProductCategories
{
    public class CreateUpdateProductCategoryDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Slug { get; set; }
        public bool IsVisibility { get; set; }
        public bool IsActive { get; set; }
        public Guid? ParentId { get; set; }
        public string? CoverPictureName { get; set; }
        public string? CoverPictureContent { get; set; }

    }
}
